# Fable notes

Explanation and reference for how `Fnode` runs under Fable. The steps are in [`fable.md`](fable.md).

## Portability rule

Where a BCL call behaves differently under Fable the code uses `#if FABLE_COMPILER` / `#if FABLE_COMPILER_PYTHON`
branches. On .NET every branch is the call the code always made, so .NET behaviour is unchanged.

## How the test harness works

xUnit and FsCheck cannot run under Fable, so `tests/Fnode.Fable` compiles the **same test files** as
`tests/Fnode.Tests` through two small shims, `XunitShim.fs` and `FsCheckShim.fs`. `scripts/fable-gen-runner.fsx`
(run by the script; `dotnet fsi`) reads those files and generates `Runner.generated.fs`: one entry per `[<Fact>]`,
`[<Theory>]` data set or `[<Property>]`, calling the test function directly. Generated files are git-ignored.
`tests/Fnode.Fable` is not in `Fnode.slnx`; the harness is also runnable on .NET
(`dotnet run --project tests/Fnode.Fable`) as a check on the shims.

**FsCheck is replaced by a shim**: a seeded xorshift generator (same sequence on every target and run, no
shrinking), with `Gen.*`, `Arb.generate` for `int`, `int64`, `bool`, `string`, `float`, `NormalFloat`,
`NonNull<string>`, and `[<Property(Arbitrary, MaxTest)>]`. Generated data differs from real FsCheck's, so the .NET
and Fable runs explore different cases. `Check.maxCases` caps cases per property.

## Why `Int of int64` stays

`Node.Int of int64` is exact on all three targets. Fable represents `int64` as a 64-bit value (JS: a `Long`
object, Python: a plain `int`), not a JS number, so nothing is lost past 2^53. Keeping `Int of int64` is
therefore safe: it is exact and identical on .NET, Fable JS and Fable Python, and `Int`/`Float` stay distinct.
Switching to `int` plus a `Float` fallback would lose exact values between 2^31 and 2^63 (ids, timestamps in ms)
for no Fable benefit. The only cost is that JS consumers see `Long` objects rather than numbers (convert at the
boundary).

## Differences between targets

- **Float text.** `1E+21` on .NET, `1e+21` on JS and Python. It is spelling, not value; do not compare float text
  across targets.
- **Python strings are code points, not UTF-16 units.** An astral character has `Length` 1, so any column
  or offset an adapter computes counts it as one unit (JS and .NET count two).
- **Parsing and comparison.** Fable's `Double.TryParse` / `Int64.TryParse` ignore `NumberStyles` and culture;
  `String.CompareOrdinal` with offsets does not honour them, so use `Substring` equality instead;
  `Double.ToString("R")` is not available.

## Gotcha: do not put a generated project in a folder called `fable`

The harness references `src/Fnode.Core` by project. If that project has a folder named `fable` holding an `.fsproj`
(for example under `obj/Release/`), Fable takes the reference for an already packed Fable package and fails with
"Files in libraries ... must begin with a namespace or module declaration". `build/Fable.targets` generates the
project that ships under `fable/` in the package, so it writes it to `obj/<config>/pack-fable-project/` instead.

## Results

Last verified 2026-11-17 (Fable 5.21.0): `dotnet test` passes 159 tests; `scripts/fable-test.sh all` passes 142
entries on Fable JS and on Fable Python. The counts differ because the runner has one entry per test function and
data set, while xUnit counts each theory case. `STATUS.md` holds the current numbers.
