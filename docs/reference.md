# Reference

The public API of `Fnode.Core`, `Fnode.Json` and `Fnode.Yaml`. All of it is in the `Fnode` namespace. Every
operation is pure, never throws, and reports failure as an `Error` value. For a walkthrough see the
[tutorial](tutorial.md).

## Types (`Fnode.Core`)

### `Node`

| Case | Holds | Notes |
|---|---|---|
| `Null` | | |
| `Bool` | `bool` | |
| `Int` | `int64` | Exact on .NET, Fable JS and Fable Python. |
| `Float` | `float` | `=` is not NaN-safe; compare with `Node.equals`. |
| `Str` | `string` | |
| `Timestamp` | `string` | The original ISO 8601 text. |
| `Arr` | `Node list` | |
| `Obj` | `(string * Node) list` | Key order is kept. Duplicate keys are allowed; lookups use the first match. |

### `Segment` and `Path`

`Segment` is `Key of string` or `Index of int`. `Path` is `Segment list`; `[]` is the document root. A path prints
and parses as a JSON Pointer (RFC 6901).

### `Parsed`

| Field | Type | Meaning |
|---|---|---|
| `Node` | `Node` | The document. |
| `Locations` | `Map<string, Span>` | Source span of each node, keyed by printed JSON Pointer. |
| `ScalarText` | `Map<string, string>` | The text each number was written as (`1.10`, `1e3`, an integer past int64). No entry means no recoverable text. |

For duplicate keys, `Locations` and `ScalarText` describe the first occurrence. A number reached through a YAML
alias reports the anchor's text.

### `Pos` and `Span`

`Pos = { Line: int; Column: int }` (1-based). `Span = { Start: Pos; End: Pos }`.

### `Error`

| Field | Type | Meaning |
|---|---|---|
| `Code` | `string` | Stable code, see below. |
| `Message` | `string` | |
| `Path` | `Path` | The requested path up to and including the failing segment. |
| `Location` | `Span option` | Source location, when known. |
| `Context` | `(string * string) list` | Ordered key/value pairs set by the producer; the core never interprets them. |

| `ErrorCode` | Value | Raised when |
|---|---|---|
| `parse` | `"parse"` | The text is not valid for the format. |
| `pathNotFound` | `"path.notFound"` | A key or index is absent. |
| `typeMismatch` | `"type.mismatch"` | A segment does not fit the node (key on an array, index on an object or scalar), or an unsupported value such as a non-string YAML key. |
| `pathInvalid` | `"path.invalid"` | A JSON Pointer is malformed. |

### `Format`

`{ Name: string; Extensions: string list; Parse: string -> Result<Parsed, Error list>; Print: Node -> string }`.
Text in, text out; no file IO. Adapters return every parse error they find.

## `Node` module

| Function | Signature | Behaviour |
|---|---|---|
| `tryGet` | `Path -> Node -> Result<Node, Error>` | The node at the path. `path.notFound` or `type.mismatch` on failure. |
| `tryFind` | `Path -> Node -> Result<Node option, Error>` | `Ok None` when absent; `Error` only on a type mismatch. |
| `exists` | `Path -> Node -> bool` | |
| `set` | `Path -> Node -> Node -> Result<Node, Error>` | `set path value node`. Replaces an existing key or index, appends a new key, or appends at an index equal to the array length. The empty path replaces the whole document. A missing parent is `path.notFound`. |
| `setCreating` | `Path -> Node -> Node -> Result<Node, Error>` | As `set`, and creates `Obj []` for missing `Key` parents. A missing array index is still an error. |
| `remove` | `Path -> Node -> Result<Node, Error>` | Removing from an array shifts later items down. Removing the root is `path.invalid`. |
| `equals` | `Node -> Node -> bool` | Structural equality that treats NaN as equal to NaN. |
| `tryFindKey` | `string -> (string * Node) list -> Node option` | First value under the key. |
| `upsertKey` | `string -> Node -> (string * Node) list -> (string * Node) list` | Replace in place or append. |
| `removeKey` | `string -> (string * Node) list -> (string * Node) list` | Order preserving. |
| `hasDuplicateKeys` | `(string * Node) list -> bool` | |

## `Path` module

| Function | Signature | Behaviour |
|---|---|---|
| `parse` | `string -> Result<Path, Error>` | A JSON Pointer to a `Path`. `""` is the root. |
| `print` | `Path -> string` | A `Path` to a JSON Pointer. |
| `parseSegment` | `string -> Result<Segment, Error>` | One escaped segment. |
| `printSegment` | `Segment -> string` | |
| `escapeSegment` | `string -> string` | `~` to `~0`, `/` to `~1`. |
| `unescapeSegment` | `string -> string option` | `None` on a `~` not followed by 0 or 1. |
| `resolve` | `Node -> Path -> Path` | Normalise a path against a document (see below). |

**Numeric keys.** A segment of digits with no leading zero and at most nine digits parses as `Index`; anything else,
including `01`, parses as `Key`. So `print [Key "5"]` re-parses as `Index 5`. `resolve` turns a `Key` of digits into
an `Index` where the node is an array, and an `Index` into a `Key` where the node is an object.

## `Error` module

| Function | Signature | Behaviour |
|---|---|---|
| `create` | `string -> string -> Path -> Error` | Code, message, path; no location, no context. |
| `withContext` | `string -> string -> Error -> Error` | Append a context entry. |
| `mapMessage` | `(string -> string) -> Error -> Error` | |
| `describe` | `Error -> string` | Outermost context first, then the message: `rule 2: input 'team': message`. |

## Adapters

| Package | Value | Name | Extensions | Built on |
|---|---|---|---|---|
| `Fnode.Json` | `Json.format` | `json` | `.json` | `Flexi.Json` (strict RFC 8259) |
| `Fnode.Yaml` | `Yaml.format` | `yaml` | `.yaml`, `.yml` | `Flexi.Yaml` |

- **Print:** JSON is pretty-printed with two-space indent. YAML prints block style.
- **Timestamps:** YAML keeps the original text in `Timestamp`. JSON has no timestamps: `Timestamp` prints as a
  string and parses back as `Str`.
- **Comments:** `Node` carries none, so they do not survive a parse and print.
- **YAML non-string mapping key:** a `type.mismatch` error that carries the location of the mapping.
- **Parse errors:** an `Error list`, each with a `Location`.

`JsonConvert` (`toNode`, `ofNode`) converts between a `Flexi.Json` value and a `Node`. `YamlConvert` (`toNode`,
`toNodeWith`, `toNodeLocated`, `ofNode`) does the same for `Flexi.Yaml`.

## Targets and versions

| | |
|---|---|
| Target frameworks | `netstandard2.0`, `net10.0` |
| Fable | JS and Python, 5.21.0; sources ship under `fable/` in each package |
| Versioning | One version for all three packages; see [`releasing.md`](releasing.md) |
