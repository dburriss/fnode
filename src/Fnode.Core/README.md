# Fnode.Core

The shared document core: one small tree, `Node`, that YAML and JSON (and other formats) both parse to, with
JSON Pointer paths, structured errors with source locations, and a `Format` interface. Pure and Fable-safe:
no dependencies, no reflection, no IO. Runs on .NET, Fable JavaScript and Fable Python from the same source.

The format adapters `Fnode.Json` and `Fnode.Yaml` live in the same repo as separate packages that depend on it.
All three release in lockstep (one tag, one version); `Fnode.Core` itself has no dependencies and works alone.

## Install

```sh
dotnet add package Fnode.Core --prerelease
```

## Example

```fsharp
open Fnode

let doc = Obj [ "name", Str "web"; "ports", Arr [ Int 80L; Int 443L ] ]

// read and change a value by JSON Pointer
Path.parse "/ports/1" |> Result.bind (fun p -> Node.tryGet p doc)       // Ok (Int 443L)
Path.parse "/ports/1" |> Result.bind (fun p -> Node.set p (Int 8443L) doc)
// Ok (Obj [ "name", Str "web"; "ports", Arr [ Int 80L; Int 8443L ] ])
```

`Node` is `Null | Bool | Int of int64 | Float | Str | Timestamp of string | Arr | Obj`. `Int` is an exact `int64`
on every target and `Timestamp` keeps the original ISO text. Operations return `Result<_, Error>`; an `Error`
has a stable code, a path, an optional source location and context entries.

## Learn more

- [Repository](https://github.com/dburriss/fnode) with the architecture notes and decisions
- Fable (JS and Python) support: [`docs/fable.md`](https://github.com/dburriss/fnode/blob/main/docs/fable.md)

MIT licensed.
