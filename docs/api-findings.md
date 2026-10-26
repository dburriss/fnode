# Fnode API findings

Findings about the `Fnode` API from the Fergi spike (delivery step 5): Fergi consumed the core by project
reference before anything was published, so mistakes in the core API cost nothing. This is the part of the
spike's findings document that is about Fnode; the rest (Fergi design, merge laws, Fergi under Fable) stays in
the Fergi repo (`docs/fergi-spike.md` there). References to `Fergi` below are the consumer that hit the friction.
The YAML and JSON adapters mentioned below are not in this repo; names are as of the spike.

## Fnode API friction (deduplicated, with fixes)

Priority: **P1** fix before publishing 0.x (public API shape, hard to change later), **P2** worth doing
before 0.x if cheap, **P3** can follow in a minor version.

1. **No public path-segment escaping.** `Path.escape`/`unescape` are private, `Path.parse` only parses a
   whole pointer. Fergi re-implemented `unescape` (and `escape`) to parse `[key=value]` selector parts.
   Fix: expose `Path.escapeSegment`, `Path.unescapeSegment : string -> string option` and
   `Path.parseSegment : string -> Result<Segment, Error>` (`Path.printSegment` is already public). **P1**.
   **Done: `Path.escapeSegment`, `Path.unescapeSegment`, `Path.parseSegment`; Fergi's `Selector` uses them.**
2. **`Node.set` does not create missing parents, and there is no existence test.** The mapping rule
   `Copy ... -> /metadata/name` failed with `path.notFound` when `/metadata` was absent; Fergi added its own
   `setCreating` (emits `Set(parent, Obj [])` ops). Fix: add `Node.setCreating : Path -> Node -> Node -> Result<Node, Error>`
   (creates `Obj []` for missing `Key` parents, still errors for a missing array index) and
   `Node.exists : Path -> Node -> bool`. **P1** (every consumer needs it).
   **Done: `Node.setCreating` and `Node.exists`. `Op.Set` stays strict, so Fergi's `Run` still emits explicit parent `Set` ops, but finds the missing parents with `Node.exists`.**
3. **`tryGet`/`set`/`remove` cannot say "absent" vs "mismatch" without reading `Error.Code`.** Fergi has
   `isAbsent e = e.Code = "path.notFound" || ...` in several places. Fix: either a total
   `Node.tryFind : Path -> Node -> Result<Node option, Error>` (Ok None = absent, Error = type mismatch), or
   a DU on the error (`ErrorKind = NotFound | Mismatch | ...`) next to the string code. **P1**.
   **Done: `Node.tryFind` (`Ok None` = absent, `Error` = mismatch). `Fergi.Run` uses it for `DefaultTo`/`Drop`.**
4. **No object helpers; `Obj` can hold duplicate keys.** Fergi hand-rolls "find key", "replace in place",
   "append key", "remove key keeping order" and `hasDuplicates` (needed because `Patch.diff` cannot be
   correct on duplicate keys, so it falls back to a whole-object `Set`). Fix: `Node.tryFindKey`,
   `Node.upsertKey`, `Node.removeKey` (order preserving), `Node.hasDuplicateKeys`; decide and document whether
   adapters may produce duplicates (YAML parse currently can). **P2**.
   **Done: `Node.tryFindKey`, `upsertKey`, `removeKey`, `hasDuplicateKeys`. Duplicate keys remain allowed in `Obj` (documented on `Node`); lookups act on the first match.**
5. **`Error.Path` is a `Path`, so it cannot carry a `Where`.** A selector error such as "no item matches
   `[name=x]`" can only report the resolved prefix; the failing step is in the message text. Fix: keep
   `Path` and document the convention (path = resolved prefix), or make `Error` generic over the path
   type. Low value to change; **P3**, mainly a documentation item.
6. **`Error` has no structured context.** Fergi puts context in message prefixes: `op 3:`, `rule 2 (Copy):`,
   `input 'team' (teams/x.yaml):`, and conflicts need two values and two source names (Fergi has its own
   `Conflict` type with `Left/Right/LeftSource/RightSource`, converted by `Conflict.toError`).
   Callers cannot get these back as data. Fix: add `Context: (string * string) list` (or a
   `Source: string option`) to `Error`, plus `Error.withContext`/`Error.mapMessage` helpers. **P2**.
   **Done: `Error.Context`, `Error.withContext`, `Error.mapMessage`, `Error.describe`. Fergi's `op`, `rule`/`kind`, `input`/`file`, `copy source` and `ensure` prefixes are now context entries; `Error.describe` renders them. `Conflict` still has its own type.**
7. **`Float nan` breaks structural equality.** `Test`, `diff`, `merge` and `Union` all use `=`, and
   `Float nan <> Float nan`, so `diff a a` is not empty and merge reports `values differ` for identical NaN
   leaves (documented in `Doc.merge`). Fix: define `Node` equality/`Node.equals` with bitwise float
   comparison (or custom equality on `Float`), or state in the core that NaN is not a legal `Node`. **P1**
   (equality is part of the contract; changing it after publishing is breaking).
   **Done: `Node.equals` (NaN-safe). Plain `=` is unchanged; Fergi's merge, diff and `Test` use `Node.equals`.**
8. **A numeric object key prints the same as an `Index`.** `Path.print [Key "0"]` and `[Index 0]` are both
   `/0`, so provenance keys (`Path.print`) and `ListRules` pointers are ambiguous between "object key 0" and
   "array item 0"; `Path.parse "/0"` always yields `Key "0"` (resolution against the document decides).
   Fix: document it as a property of JSON Pointer, and offer `Path.resolve : Node -> Path -> Path` (turn
   numeric `Key` into `Index` when the node is an array) so consumers can normalise. **P2**.
   **Done: `Path.resolve` normalises a path against a document; the JSON Pointer ambiguity itself is documented in `Path.fs`.**
9. **Path equality in xunit assertions needs `Assert.True(a = b)`.** `Path` is a plain list alias, so
   xunit's `Assert.Equal` can bind to a sequence overload; the tests use
   `Assert.True((ptr "/team" = c.Path))` or `Assert.Equal<Path>`. Fix: make `Path` a single-case record/struct
   (`Path of Segment list`) with its own equality and `ToString`, or document `Assert.Equal<Path>`. **P3**.
10. **The loader shape `string -> Result<Node, Error>` lost information. Fixed (follow-up [b]).** It kept only
    the first adapter error (`Format.Parse` returns `Error list`) and dropped `Parsed.Locations`. The loader is
    now `Loader = string -> Result<Parsed, Error list>` (exactly `Format.Parse`'s result), so every parse error
    survives (each prefixed `input 'x' (file): `, Path and Location kept) and merge conflicts are tied to
    `file:line:col` when the source recorded a span for the conflict pointer (right source first, then left;
    otherwise `Location = None`). No Fnode change was needed. `Loader.ofNodes` wraps in-memory nodes (empty
    Locations) for tests. Limit: `Copy` `path.notFound` errors are not located (a missing path has no span).
11. **Fable: expected-type union-case resolution differs from the .NET compiler.** Not an Fnode issue; see 5.
