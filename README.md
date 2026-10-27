# Fnode

The shared document core in F#: one small tree, `Node`, that formats such as YAML and JSON can parse to, with
JSON Pointer paths, errors that carry source locations, and a `Format` interface. Pure and Fable-safe: no
dependencies, no reflection, no IO. It compiles to .NET, Fable JavaScript and Fable Python from the same source.

This repo holds the core (`Fnode.Core`) and the format adapters `Fnode.Json` and `Fnode.Yaml`, each its own package.
The adapters depend on the core and on the `Flexi.Json` / `Flexi.Yaml` NuGet packages; the core has no dependencies.
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
src/Fnode.Core     the core package
src/Fnode.Json     the JSON adapter package
src/Fnode.Yaml     the YAML adapter package
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

## Releases

`Fnode.Core`, `Fnode.Json` and `Fnode.Yaml` release in lockstep: one `v*` tag packs and publishes all three at the
same version, so `Fnode.Json 0.2.0` always depends on `Fnode.Core 0.2.0`. They are still separate packages and each
can be used alone:

- `Fnode.Core` has no dependencies.
- `Fnode.Json` needs only `Fnode.Core` and `Flexi.Json`.
- `Fnode.Yaml` needs only `Fnode.Core` and `Flexi.Yaml`.

To release, run `dotnet fsi scripts/publish.fsx --fsproj Directory.Build.props --solution Fnode.slnx` (it bumps the
one `<Version>` in `Directory.Build.props`, updates `CHANGELOG.md`, commits and tags) and push the tag. The tag then
starts the GitHub Actions workflows: `publish-nuget.yml` checks the tag against `<Version>`, runs `scripts/pack.sh`
(the one list of packages; it fails if any package is missing or not in `Fnode.slnx`) and pushes every nupkg to
nuget.org through Trusted Publishing; `github-release.yml` attaches the same nupkgs to a GitHub Release. A core-only
change still releases the adapters.

## License

MIT, see [`LICENSE`](LICENSE).
