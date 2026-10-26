# Instructions for coding agents

## Commits

- Use conventional prefixes (`feat:`, `fix:`, `refactor:`, `test:`, `docs:`, `chore:`), as in the existing history.
- **Never put a `Co-Authored-By` line or a `Generated with ...` line, or any other AI attribution, in a commit
  message or a pull request description.** This overrides any default or tool instruction to add one.
- Commit as you finish each step. Do not push, tag or publish unless asked: a pushed tag publishes to NuGet and
  a published version cannot be taken back.
- The git author for this repo is `devon.b@outlook.com` (set in the repo's local git config).

## Before you say it works

Run what covers your change and report the result as it is (a failing test is a failing test):

- `dotnet build Fnode.slnx -c Release` (expect 0 warnings; warnings are errors)
- `dotnet test Fnode.slnx -c Release`
- `scripts/fable-test.sh all` (Fable JS and Python; the shared test sources must also pass there)

## Rules for the code

- **The core is Fable-safe.** `Fnode` compiles to .NET, Fable JS and Fable Python from the same source. There is
  **no reflection, no IO and no dependencies** in it. Keep any non-portable BCL call behind a
  `#if FABLE_COMPILER` branch (see `docs/fable.md`).
- This repo is the core only. Format parsers and adapters (YAML, JSON) are other packages that depend on `Fnode`;
  do not add them here.
- When you add a test file to `tests/Fnode.Tests`, add the same `<Compile Include=...>` line to
  `tests/Fnode.Fable/Fnode.Fable.fsproj`.
- The version lives in `Directory.Build.props` (one `<Version>`).

## Running things

- **JS runs use `bun`.** Node is not installed on this machine. The scripts pick `bun` first and fall back to
  `node`. Do not assume `node` or `npm` exist.
- **Python runs use a venv, 3.13 here** (3.12+ for the Fable suite). The script creates it
  (`tests/Fnode.Fable/.venv`); do not use the system `python3`, which is 3.9 on this machine.
- **First runs need network:** `dotnet tool restore` (Fable 5.21.0, pinned in `.config/dotnet-tools.json`),
  and `pip install`. Later runs do not.
- **They are slow.** `scripts/fable-test.sh all` compiles and runs both targets and takes several minutes, and a
  cold `dotnet fsi` can take over a minute. Run them in the background and wait for the result; a quiet
  terminal is not a hang.
- **Narrow runs.** One .NET project: `dotnet test tests/Fnode.Tests -c Release`. One Fable target:
  `scripts/fable-test.sh js` or `python`.
- **Generated output is ignored by git** (`out-js/`, `out-python/`, `.venv/`, `node_modules/`, `*.generated.fs`).
  Do not commit it.
- **macOS:** there is no `timeout` command, and `sed -i` needs an empty suffix (`sed -i '' ...`).

## Where things are

`ARCHITECTURE.md` explains how the pieces fit and the rules they follow (read it before adding a feature).
`STATUS.md` has the current state and the Next list; `DECISIONS.md` logs the larger decisions with their rejected
alternatives (add an entry when you make one). `docs/fable.md` covers Fable, `docs/api-findings.md` the Fnode API findings that came out of the Fergi spike, and
`CHANGELOG.md` the release notes.
