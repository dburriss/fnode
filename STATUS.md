# Status

Snapshot of the Fnode core. How it fits together is in `ARCHITECTURE.md`; decisions are in `DECISIONS.md`; Fable
notes are in `docs/fable.md`; the API findings that shaped the core are in `docs/api-findings.md`; rules for
coding agents are in `AGENTS.md`.

Last verified: 2026-10-26. `dotnet build Fnode.slnx -c Release` has 0 warnings, `dotnet test` passes 30 tests, and `scripts/fable-test.sh all` passes 30 entries on Fable JS and on Fable Python. Nothing is published.

## What is in this repo

| Project | Purpose |
|---|---|
| `src/Fnode.Core` | Core: `Node`, `Path`, `Error`, `Parsed`, `Format`, `Node.tryGet/tryFind/exists/set/setCreating/remove`, key helpers, `Node.equals`. No dependencies |
| `tests/Fnode.Tests` | Unit and property tests |
| `tests/Fnode.Fable` | The same tests under Fable JS and Python |

## Decisions made

- `Int of int64` stays. Under Fable int64 is exact on JS and Python.
- `Timestamp of string` holds the original ISO text. The core is pure and Fable-safe, with no reflection, IO or dependencies.
- `Where` is not in core `Path`; it is a selector concept for consumers.
- Commits: conventional prefixes, no AI attribution lines.

## Known limits

- `Node` carries no comments.
- Environment: Node is not installed here, so JS runs use bun. Python runs use a 3.13 venv and need network on first run.
- The test projects use VSTest. `scripts/publish.fsx` runs `dotnet test --solution`, which needs Microsoft.Testing.Platform,
  so its preflight test step fails until the tests move to MTP. CI uses plain `dotnet test` and works.

## Next

1. [x] Priority-1 and priority-2 API fixes (see `docs/api-findings.md`).
2. [ ] Publish the first `Fnode.Core` version: `NUGET_USER` secret, then run `./publish.sh` (versions restart from `0.0.0`).
3. [ ] Request the `Fnode.` package id prefix reservation on nuget.org; pick npm and PyPI names (`fnode` is taken on npm).
4. [ ] Adapters (YAML, JSON) and consumers (Fergi, Falid) take the package as a dependency.
