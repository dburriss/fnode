# Architecture

How the piece fits and the rules it follows. `STATUS.md` has the current state; `DECISIONS.md` has the
reasoning behind the larger choices.

## Where Fnode sits

```
 files (YAML / JSON / ...)   what people and programs read and write
        |
 format adapters             Fnode.Json, Fnode.Yaml: text <-> Node, with source locations
        |
 Fnode                       Node, Path, Error, Parsed, Format: the shared core, no dependencies
```

This repo holds the bottom two layers: the core and the Json and Yaml adapters, each its own project and package.
The adapters depend on the core by project reference and on `Flexi.Json` / `Flexi.Yaml` by `PackageReference`
only; the core depends on nothing. A format adapter implements `Format` and fills `Parsed.Locations`; a
consumer (a merge or mapping library, a linter) works on `Node` and never needs to know which format the
document came from. Each layer depends only on the ones below it.

## Principles

### `Node` is the working form; the format lives at the edges

Every format parses to the same `Node` tree. Going through one format's text to reach another would add nothing
and lose locations, timestamp text and format-only values. Cost: `Node` carries no comments, so they do not
survive a round trip through an adapter.

### The core is pure and Fable-safe

`Fnode` has no IO, no reflection and no dependencies the Fable targets cannot compile, so the same source runs
on .NET, Fable JS and Fable Python, and the tests run on all three. The package ships its F# sources under
`fable/` so a Fable consumer compiles them from a plain `PackageReference`.

### Failures are values

Operations return `Result<_, Error>`. An `Error` has a stable code, a path, an optional source location and
context entries (`Error.withContext`). Adapters return every error they find as an `Error list`.

### `Int of int64` and original timestamp text

`Node.Int` is an exact `int64` on all three targets (values past 2^53 survive). `Node.Timestamp` holds the
original ISO text, so a date stays a date and a zoneless date-time stays zoneless through a round trip. See
`docs/fable.md`.

## Tests

`tests/Fnode.Tests` (xUnit and FsCheck). `tests/Fnode.Fable` compiles the same test files under Fable through
small xUnit and FsCheck shims; `scripts/fable-gen-runner.fsx` generates its runner.
