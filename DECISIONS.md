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

## This repo is the core only
Date: 2026-10-26. Status: Accepted.

Decision: `dburriss/fnode` holds the `Fnode.Core` package and nothing else: the `Node` tree, `Path`, `Error`,
`Parsed`, `Format` and the `Node` operations. The YAML and JSON parsers and adapters are not in this repo.

Why: the core is the one piece other libraries share, so it should be small, dependency-free and released on its
own. Parsers are a separate concern with their own history and release rhythm.

Rejected:
- **Putting the YAML parser (formerly `FSharp.Data.YamlValue`) and the JSON parser (formerly `JsonValue`) in this
  repo as `Fnode.Yaml` and `Fnode.Json`**: tried first, then backed out. It made the repo mostly parser code.
- **Carrying the Fergi history**: the core had almost no history of its own (one commit that touched only it),
  so the repo starts fresh from the current code.

Revisit if: adapters turn out to need lockstep releases with the core.

## The package is Fnode.Core
Date: 2026-10-26. Status: Accepted.

Decision: the package id, project and assembly are `Fnode.Core`; the root namespace stays `Fnode`, so adapters
write `open Fnode`. The repo stays `dburriss/fnode`. An earlier `Fnode` 0.1.0 package was published and is
unlisted and deprecated in favour of `Fnode.Core`.

Why: the adapters are `Fnode.JsonAdapter` and `Fnode.YamlAdapter`, and the core is the package they share, so
`Fnode.Core` reads as the base of the family and leaves `Fnode` free as a prefix.

Rejected: renaming the namespace to `Fnode.Core` too (`Fnode.Core.Node` reads badly and every consumer opens a
longer name); keeping `Fnode` as the package id (it does not match the sibling names).

Revisit if: an umbrella `Fnode` package is wanted.
