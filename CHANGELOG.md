# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- `Parsed.ScalarText`: the text each `Int`/`Float` node was written as in the source (`1.10`, `1.0`, `1e3`, an integer
  beyond int64), keyed by JSON Pointer like `Locations`. `Fnode.Json` and `Fnode.Yaml` fill it from `Flexi.Json` /
  `Flexi.Yaml` 0.2.0. A pointer with no entry has no recoverable text (an aliased YAML number reports the anchor's
  text); use the `Node` value.
- `YamlConvert.toNodeLocated`, `toNodeWith` plus the spans, so the non-string-key error can carry a location.

### Fixed

- The `Fnode.Yaml` `type.mismatch` error for a non-string mapping key now has a `Location` (the span of the mapping).

### Changed

- **Breaking:** `Parsed` has a new field, `ScalarText`, so code that builds a `Parsed` record literal must set it
  (`ScalarText = Map.empty`). Reading fields and `{ p with ... }` are unaffected.
- `Fnode.Json` and `Fnode.Yaml` need `Flexi.Json` / `Flexi.Yaml` 0.2.1 or newer.
- The packed FSharp.Core dependency of all three packages is now an explicit minimum, 8.0.403, instead of the SDK
  default. (Flexi 0.2.0 declared 11.0.101 by accident, because the CI SDK's default had moved; 0.2.1 fixes that.)
- For duplicate object keys, `Node` keeps every pair (lookups take the first); `Locations` and `ScalarText` describe the
  first occurrence only. Documented, behaviour unchanged.

- Target frameworks are now `netstandard2.0` and `net10.0` (was `netstandard2.0` and `net8.0`; .NET 8 is out of LTS).
  Projects on .NET 8 or 9 keep working through the `netstandard2.0` assets.

## [0.1.0] - 2026-10-27

### Added

- `Fnode.Json` (JSON adapter on `Flexi.Json`) and `Fnode.Yaml` (YAML adapter on `Flexi.Yaml`) packages, each
  usable alone: `Fnode.Json` needs only `Fnode.Core` and `Flexi.Json`, `Fnode.Yaml` only `Fnode.Core` and
  `Flexi.Yaml`.
- Lockstep releases: one `v*` tag packs and publishes `Fnode.Core`, `Fnode.Json` and `Fnode.Yaml` at the same
  version (`scripts/pack.sh` holds the package list and fails if a package is missing), with each adapter
  depending on `Fnode.Core` at that exact version. The workflows also check the tag against `Directory.Build.props`.

- `Fnode.Core`: the shared document core. The `Node` tree (`Int` is an exact `int64`, `Timestamp` keeps the original
  ISO text), JSON Pointer `Path` with `Path.escapeSegment`/`unescapeSegment`/`parseSegment`/`resolve`, `Error`
  with structured context (`withContext`, `mapMessage`, `describe`), `Parsed` (the tree plus source locations)
  and `Format`. `Node.tryGet`, `tryFind`, `exists`, `set`, `setCreating`, `remove`, the key helpers
  (`tryFindKey`, `upsertKey`, `removeKey`, `hasDuplicateKeys`) and the NaN-safe `Node.equals`. No dependencies,
  no reflection, no IO.
- The package ships its F# sources under `fable/`, so Fable JavaScript and Python projects can use a plain
  `PackageReference`.
