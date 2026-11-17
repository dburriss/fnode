# Fnode

The shared document core in F#: one small tree, `Node`, that formats such as YAML and JSON can parse to, with
JSON Pointer paths, errors that carry source locations, and a `Format` interface. Pure and Fable-safe: no
dependencies, no reflection, no IO. It compiles to .NET, Fable JavaScript and Fable Python from the same source.

This repo holds the core (`Fnode.Core`) and the format adapters `Fnode.Json` and `Fnode.Yaml`, each its own package.
The adapters depend on the core and on the `Flexi.Json` / `Flexi.Yaml` NuGet packages; the core has no dependencies.
Status: pre-release. The packages are on nuget.org; `STATUS.md` has the current version and what is next.

```fsharp
open Fnode

let doc = Obj [ "name", Str "web"; "ports", Arr [ Int 80L; Int 443L ] ]

Path.parse "/ports/1" |> Result.bind (fun p -> Node.tryGet p doc)       // Ok (Int 443L)
Path.parse "/ports/1" |> Result.bind (fun p -> Node.set p (Int 8443L) doc)
// Ok (Obj [ "name", Str "web"; "ports", Arr [ Int 80L; Int 8443L ] ])
```

## Layout

```
src/Fnode.Core     the core package
src/Fnode.Json     the JSON adapter package
src/Fnode.Yaml     the YAML adapter package
tests/Fnode.Tests  unit and property tests (xUnit, FsCheck)
tests/Fnode.Fable  the same tests under Fable JS and Python
docs/              tutorial, reference, Fable, releasing, the Fnode API findings
```

New here? Do the [tutorial](docs/tutorial.md); look things up in the [reference](docs/reference.md). Then
[`ARCHITECTURE.md`](ARCHITECTURE.md) for how it fits together, [`DECISIONS.md`](DECISIONS.md) for why,
and [`STATUS.md`](STATUS.md) for where things stand.

## Build and test

```bash
dotnet build Fnode.slnx -c Release
dotnet test Fnode.slnx -c Release
scripts/fable-test.sh all        # the tests under Fable JS and Python (slow; needs bun or node, Python 3.12+)
```

The .NET SDK is 10; the library targets `netstandard2.0` and `net10.0`. Fable is pinned to 5.21.0 in
`.config/dotnet-tools.json`. The package ships its F# sources under `fable/`, so a Fable project needs only a
`PackageReference`; see [`docs/fable.md`](docs/fable.md) (notes: [`docs/fable-notes.md`](docs/fable-notes.md)).

## Releases

The three packages release in lockstep (one `v*` tag, one version) and each can be used alone. Steps:
[`docs/releasing.md`](docs/releasing.md).

## License

MIT, see [`LICENSE`](LICENSE).
