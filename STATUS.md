# Status

Snapshot of the Fnode core and its Json and Yaml adapters. How it fits together is in `ARCHITECTURE.md`; decisions are in `DECISIONS.md`; Fable
notes are in `docs/fable.md`; the API findings that shaped the core are in `docs/api-findings.md`; rules for
coding agents are in `AGENTS.md`.

Last verified: 2026-11-17. `dotnet build Fnode.slnx -c Release` has 0 warnings, `dotnet test` passes 159 tests, and `scripts/fable-test.sh all` passes 142 entries on Fable JS and on Fable Python. The 0.2.0 changes (`Parsed.ScalarText`, the Yaml non-string-key location, the net10.0 retarget) are committed on `main` but not released; 0.1.0 is the last published version.

## What is in this repo

| Project | Purpose |
|---|---|
| `src/Fnode.Core` | Core: `Node`, `Path`, `Error`, `Parsed` (with `Locations` and `ScalarText`), `Format`, `Node.tryGet/tryFind/exists/set/setCreating/remove`, key helpers, `Node.equals`. No dependencies |
| `src/Fnode.Json` | JSON adapter (`Format`). Depends on `Fnode.Core` (project) and `Flexi.Json` (NuGet) |
| `src/Fnode.Yaml` | YAML adapter (`Format`). Depends on `Fnode.Core` (project) and `Flexi.Yaml` (NuGet) |
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
2. [x] Publish the first lockstep release (`Fnode.Core`, `Fnode.Json`, `Fnode.Yaml` 0.1.0, on nuget.org, tag `v0.1.0`).
   `scripts/pack.sh` refuses to release a package that is missing from `Fnode.slnx`.
3. [ ] Release 0.2.0 (`Parsed.ScalarText`, the Yaml non-string-key location, net10.0, an explicit FSharp.Core 8.0.403 floor, on
   `Flexi.Json` / `Flexi.Yaml` 0.2.1): everything is committed on `main` and verified; what is left is `scripts/publish.fsx`, the tag and the push, then
   a look at the CI-built nuspecs.
4. [ ] Request the `Fnode.` package id prefix reservation on nuget.org; pick npm and PyPI names (`fnode` is taken on npm).
5. [ ] Consumers (Config, Fergi, Falid) take `Fnode.Core` and the adapters as package dependencies.
