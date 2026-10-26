# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- The package id is now `Fnode.Core`, matching the `Fnode.JsonAdapter` and `Fnode.YamlAdapter` packages that depend on
  it. The namespace stays `Fnode`. The `Fnode` package (0.1.0) is superseded: use `Fnode.Core`.

## [0.1.0] - 2026-10-26

### Added

- `Fnode`: the shared document core. The `Node` tree (`Int` is an exact `int64`, `Timestamp` keeps the original
  ISO text), JSON Pointer `Path` with `Path.escapeSegment`/`unescapeSegment`/`parseSegment`/`resolve`, `Error`
  with structured context (`withContext`, `mapMessage`, `describe`), `Parsed` (the tree plus source locations)
  and `Format`. `Node.tryGet`, `tryFind`, `exists`, `set`, `setCreating`, `remove`, the key helpers
  (`tryFindKey`, `upsertKey`, `removeKey`, `hasDuplicateKeys`) and the NaN-safe `Node.equals`. No dependencies,
  no reflection, no IO.
- The package ships its F# sources under `fable/`, so Fable JavaScript and Python projects can use a plain
  `PackageReference`.
