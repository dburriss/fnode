# Fnode

The shared document core in F#: one small tree, `Node`, that formats such as YAML and JSON can parse to, with
JSON Pointer paths, errors that carry source locations, and a `Format` interface. Pure and Fable-safe: no
dependencies, no reflection, no IO. It compiles to .NET, Fable JavaScript and Fable Python from the same source.

This repo holds only the core, the `Fnode.Core` package. Format adapters (YAML, JSON) live elsewhere and depend on it.
Status: pre-release, nothing is published yet under the `Fnode.Core` name.

```fsharp
open Fnode

let doc = Obj [ "name", Str "web"; "ports", Arr [ Int 80L; Int 443L ] ]

Path.parse "/ports/1" |> Result.bind (fun p -> Node.tryGet p doc)       // Ok (Int 443L)
Path.parse "/ports/1" |> Result.bind (fun p -> Node.set p (Int 8443L) doc)
// Ok (Obj [ "name", Str "web"; "ports", Arr [ Int 80L; Int 8443L ] ])
```

## Layout

```
src/Fnode.Core     the package
tests/Fnode.Tests  unit and property tests (xUnit, FsCheck)
tests/Fnode.Fable  the same tests under Fable JS and Python
docs/              Fable notes, the Fnode API findings
```

Start with [`ARCHITECTURE.md`](ARCHITECTURE.md) for how it fits together, [`DECISIONS.md`](DECISIONS.md) for why,
and [`STATUS.md`](STATUS.md) for where things stand.

## Build and test

```bash
dotnet build Fnode.slnx -c Release
dotnet test Fnode.slnx -c Release
scripts/fable-test.sh all        # the tests under Fable JS and Python (slow; needs bun or node, Python 3.12+)
```

The .NET SDK is 10; the library targets `netstandard2.0` and `net8.0`. Fable is pinned to 5.21.0 in
`.config/dotnet-tools.json`. The package ships its F# sources under `fable/`, so a Fable project needs only a
`PackageReference`; see [`docs/fable.md`](docs/fable.md).

## License

MIT, see [`LICENSE`](LICENSE).
