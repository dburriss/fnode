# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
