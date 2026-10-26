# Fable support

`Fnode` compiles with [Fable](https://fable.io) 5.21.0 to JavaScript and Python from the same source (no separate
files). Where a BCL call behaves differently under Fable the code uses `#if FABLE_COMPILER` /
`#if FABLE_COMPILER_PYTHON` branches; on .NET every branch is the call the code always made, so .NET behaviour is
unchanged.

The package ships its F# sources and a project file under `fable/` (in compile order, next to the `lib/` DLLs),
which is what Fable compiles from. A plain package reference is enough:

```xml
<PackageReference Include="Fnode" Version="0.1.0-alpha.1" />
```

```bash
dotnet fable MyApp.fsproj --lang js
dotnet fable MyApp.fsproj --lang python   # then: pip install fable-library==<your Fable version>
```

## Running the test suite under Fable

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

### How it works

xUnit and FsCheck cannot run under Fable, so `tests/Fnode.Fable` compiles the **same test files** as
`tests/Fnode.Tests` through two small shims, `XunitShim.fs` and `FsCheckShim.fs`. `scripts/fable-gen-runner.fsx`
(run by the script; `dotnet fsi`) reads those files and generates `Runner.generated.fs`: one entry per `[<Fact>]`,
`[<Theory>]` data set or `[<Property>]`, calling the test function directly. Generated files are git-ignored.
`tests/Fnode.Fable` is not in `Fnode.slnx`; the harness is also runnable on .NET
(`dotnet run --project tests/Fnode.Fable`) as a check on the shims.

When you add a test file to `tests/Fnode.Tests`, add the same `<Compile Include=...>` line to
`tests/Fnode.Fable/Fnode.Fable.fsproj`.

**FsCheck is replaced by a shim**: a seeded xorshift generator (same sequence on every target and run, no
shrinking), with `Gen.*`, `Arb.generate` for `int`, `int64`, `bool`, `string`, `float`, `NormalFloat`,
`NonNull<string>`, and `[<Property(Arbitrary, MaxTest)>]`. Generated data differs from real FsCheck's, so the .NET
and Fable runs explore different cases. `Check.maxCases` caps cases per property.

### Gotcha: do not put a generated project in a folder called `fable`

The harness references `src/Fnode` by project. If that project has a folder named `fable` holding an `.fsproj`
(for example under `obj/Release/`), Fable takes the reference for an already packed Fable package and fails with
"Files in libraries ... must begin with a namespace or module declaration". `build/Fable.targets` generates the
project that ships under `fable/` in the package, so it writes it to `obj/<config>/pack-fable-project/` instead.

### Results (Fable 5.21.0)

| Project | .NET (xUnit) | Fable JS | Fable Python |
|---|---|---|---|
| Fnode.Tests | 30 | 30 | 30 |

## Numbers under Fable (the `Int of int64` question)

`Node.Int of int64` is exact on all three targets. Fable represents `int64` as a 64-bit value (JS: a `Long`
object, Python: a plain `int`), not a JS number, so nothing is lost past 2^53. Keeping `Int of int64` is
therefore safe: it is exact and identical on .NET, Fable JS and Fable Python, and `Int`/`Float` stay distinct.
Switching to `int` plus a `Float` fallback would lose exact values between 2^31 and 2^63 (ids, timestamps in ms)
for no Fable benefit. The only cost is that JS consumers see `Long` objects rather than numbers (convert at the
boundary). Float text is the only visible difference between targets, and it is spelling, not value (`1E+21` on
.NET, `1e+21` on JS and Python); do not compare float text across targets.

## Other differences

- **Python strings are code points, not UTF-16 units.** An astral character has `Length` 1, so any column
  or offset an adapter computes counts it as one unit (JS and .NET count two).
- Fable's `Double.TryParse` / `Int64.TryParse` ignore `NumberStyles` and culture; `String.CompareOrdinal` with
  offsets does not honour them, so use `Substring` equality instead; `Double.ToString("R")` is not available.
