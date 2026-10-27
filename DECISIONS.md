# Decisions

A light log of decisions that would be expensive to rediscover. Newest first. Each entry is a few lines:
what we decided, why, what we rejected, and what would make us revisit it. It is not a design document; link
to one when there is one. `STATUS.md` holds the current state and the Next list, and the shorter decisions
that are plain facts about the code.

To add one, copy the shape below and put it at the top. Change `Status` rather than deleting an entry that is
no longer true (`Superseded by <title>`), so the reasoning stays findable.

```
## <short title>
Date: YYYY-MM-DD. Status: Accepted | Spike | Superseded by <title>.
Decision: one or two sentences.
Why: the constraints and the evidence.
Rejected: option, and why not.
Costs: what we accepted by choosing this.
Revisit if: the signals that should reopen it.
See: files, docs or commits.
```

## This repo holds the core and the Json and Yaml adapters
Date: 2026-10-26. Status: Accepted.

Decision: `dburriss/fnode` holds the `Fnode.Core` package plus the format adapters `Fnode.Json` and `Fnode.Yaml`.
Each is its own project under `src/` and its own NuGet package, versioned and released in lockstep. The adapters
depend on `Fnode.Core` by project reference. The `Flexi.Json` and `Flexi.Yaml` packages (their own repos, their
own releases) are consumed as NuGet packages only (`PackageReference`); their code is not in this repo.
`Fnode.Core` itself stays dependency-free and Fable-safe.

Why: the adapters are thin and track the core API, so a change to `Format`, `Parsed` or `Error` and the adapter
change that follows it belong in one commit and one release, not in a round of cross-repo releases.

Rejected:
- **Putting the Flexi.Json and Flexi.Yaml libraries' code inside this repo**: that is the parser code backed out
  earlier (see the superseded entry below); it would make the repo mostly parser code and fork their history.
- **Separate repos per adapter**: the adapters are thin and move with the core API, so separate repos would mean
  releasing the core first and then each adapter, for every API change.

Costs: a core-only change still releases the adapters (lockstep).
Revisit if: an adapter grows its own large surface or release rhythm, or the core stops changing with them.
See: the superseded entry below.

## This repo is the core only
Date: 2026-10-26. Status: Superseded by This repo holds the core and the Json and Yaml adapters.

Decision: `dburriss/fnode` holds the `Fnode.Core` package and nothing else: the `Node` tree, `Path`, `Error`,
`Parsed`, `Format` and the `Node` operations. The YAML and JSON parsers and adapters are not in this repo.

Why: the core is the one piece other libraries share, so it should be small, dependency-free and released on its
own. Parsers are a separate concern with their own history and release rhythm.

Rejected:
- **Putting the YAML parser (formerly `FSharp.Data.YamlValue`) and the JSON parser (formerly `JsonValue`) in this
  repo as `Fnode.Yaml` and `Fnode.Json`**: tried first, then backed out. It made the repo mostly parser code.
- **Carrying the Fergi history**: the core had almost no history of its own (one commit that touched only it),
  so the repo starts fresh from the current code.

Revisit if: adapters turn out to need lockstep releases with the core. (They do; superseded above.)

## The package is Fnode.Core
Date: 2026-10-26. Status: Accepted.

Decision: the package id, project and assembly are `Fnode.Core`; the root namespace stays `Fnode`, so adapters
write `open Fnode`. The repo stays `dburriss/fnode`. An earlier `Fnode` 0.1.0 package was published and is
unlisted and deprecated in favour of `Fnode.Core`.

Why: the adapters were then named `Fnode.JsonAdapter` and `Fnode.YamlAdapter` (now `Fnode.Json` and `Fnode.Yaml`), and the core is the package they share, so
`Fnode.Core` reads as the base of the family and leaves `Fnode` free as a prefix.

Rejected: renaming the namespace to `Fnode.Core` too (`Fnode.Core.Node` reads badly and every consumer opens a
longer name); keeping `Fnode` as the package id (it does not match the sibling names).

Revisit if: an umbrella `Fnode` package is wanted.
