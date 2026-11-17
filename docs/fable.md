# Use Fnode under Fable, and run its tests under Fable

How-to guide. For why it works this way and where the targets differ, see [`fable-notes.md`](fable-notes.md).

## Compile a project that uses Fnode

`Fnode` compiles with [Fable](https://fable.io) 5.21.0 to JavaScript and Python from the same source. The package
ships its F# sources and a project file under `fable/`, so a plain package reference is enough:

```xml
<PackageReference Include="Fnode.Core" Version="x.y.z" />
```

```bash
dotnet fable MyApp.fsproj --lang js
dotnet fable MyApp.fsproj --lang python   # then: pip install fable-library==<your Fable version>
```

## Run the test suite under Fable

```bash
scripts/fable-test.sh          # JS and Python
scripts/fable-test.sh js
scripts/fable-test.sh python
```

The script exits non-zero when any test fails or the run crashes (CI friendly).

Prerequisites:

- .NET SDK 8+ (the Fable tool is pinned to 5.21.0 in `.config/dotnet-tools.json`; the script runs `dotnet tool restore`)
- JS: `bun` or `node` (anything that runs ES modules). Tested with bun 1.4
- Python: 3.12+ (3.13 tested). The script creates `tests/Fnode.Fable/.venv` and installs `fable-library==5.21.0` (network needed the first time)

## Add a test file

Add the file to `tests/Fnode.Tests`, then add the same `<Compile Include=...>` line to
`tests/Fnode.Fable/Fnode.Fable.fsproj`. The Fable runner is generated from the listed files.
