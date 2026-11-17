<p align="center">
  <img src="docs/images/fnode-logo.svg" alt="fnode" width="280">
</p>

<h3 align="center">The shared document tree for F#: one <code>Node</code> that YAML and JSON parse to.</h3>

<p align="center">
  Fnode is a small tree, <code>Node</code>, that formats such as YAML and JSON parse to, with JSON Pointer paths,
  errors that carry source locations, and a <code>Format</code> interface.
  Pure and Fable-safe: no dependencies, no reflection, no IO. .NET, Fable JavaScript and Fable Python from the same source.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/Fnode.Core"><img src="https://img.shields.io/nuget/vpre/Fnode.Core?logo=nuget&label=Fnode.Core" alt="NuGet Fnode.Core"></a>
  <a href="https://www.nuget.org/packages/Fnode.Json"><img src="https://img.shields.io/nuget/vpre/Fnode.Json?logo=nuget&label=Fnode.Json" alt="NuGet Fnode.Json"></a>
  <a href="https://www.nuget.org/packages/Fnode.Yaml"><img src="https://img.shields.io/nuget/vpre/Fnode.Yaml?logo=nuget&label=Fnode.Yaml" alt="NuGet Fnode.Yaml"></a>
  <a href="https://github.com/dburriss/fnode/blob/main/LICENSE"><img src="https://img.shields.io/github/license/dburriss/fnode" alt="License"></a>
</p>

## What it does

- **One tree.** `Node` is `Null | Bool | Int | Float | Str | Timestamp | Arr | Obj`. Every format parses to it, so a consumer never needs to know which format a document came from. See the [reference](docs/reference.md).
- **Paths.** JSON Pointer (RFC 6901) paths with `tryGet`, `tryFind`, `set`, `setCreating` and `remove`. Nothing is mutated and nothing throws.
- **Source locations.** `Parsed.Locations` gives the line and column of every node, and `Parsed.ScalarText` the text a number was written as.
- **Exact values.** `Int` is an exact `int64` on every target, and `Timestamp` keeps the original ISO text.
- **Failures are values.** Every operation returns a `Result` with an `Error` that has a stable code, a path, a location and context.
- **Adapters.** `Fnode.Json` and `Fnode.Yaml` are separate packages built on `Flexi.Json` and `Flexi.Yaml`. The core has no dependencies.
- **Fable.** Compiles to JavaScript and Python from the same source. See [Fable](docs/fable.md).

## Get started

Add the packages you need to an F# project (pre-release, hence `--prerelease`; the libraries target `netstandard2.0` and `net10.0`):

```bash
dotnet add package Fnode.Core --prerelease   # the core, no dependencies
dotnet add package Fnode.Yaml --prerelease   # YAML adapter (depends on Fnode.Core)
dotnet add package Fnode.Json --prerelease   # JSON adapter (depends on Fnode.Core)
```

Then build a document and address it by path:

```fsharp
open Fnode

let doc = Obj [ "name", Str "web"; "ports", Arr [ Int 80L; Int 443L ] ]

Path.parse "/ports/1" |> Result.bind (fun p -> Node.tryGet p doc)       // Ok (Int 443L)
Path.parse "/ports/1" |> Result.bind (fun p -> Node.set p (Int 8443L) doc)
// Ok (Obj [ "name", Str "web"; "ports", Arr [ Int 80L; Int 8443L ] ])
```

The [tutorial](docs/tutorial.md) walks through parsing, reading, changing and printing a YAML document. The three
packages release in lockstep (one version, one tag) and each can be used alone.

## Documentation

Organised by [Diataxis](https://diataxis.fr/):

| I want to... | Go to |
| --- | --- |
| Learn Fnode by doing | [Read and change a YAML document](docs/tutorial.md) |
| Get a specific job done | [Use Fnode under Fable](docs/fable.md), [Release the packages](docs/releasing.md) |
| Look up a function or type | [Reference](docs/reference.md) |
| Understand why it works this way | [Architecture](ARCHITECTURE.md), [Decisions](DECISIONS.md), [Fable notes](docs/fable-notes.md), [API findings](docs/api-findings.md) |

More: [status](STATUS.md) · [changelog](CHANGELOG.md) · [contributing](AGENTS.md)

## License

MIT, see [`LICENSE`](LICENSE).
