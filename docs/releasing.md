# Release the packages

How-to guide. `Fnode.Core`, `Fnode.Json` and `Fnode.Yaml` release in lockstep: one `v*` tag packs and publishes all
three at the same version, so `Fnode.Json 0.2.0` always depends on `Fnode.Core 0.2.0`. A core-only change still
releases the adapters.

1. Run `dotnet fsi scripts/publish.fsx --fsproj Directory.Build.props --solution Fnode.slnx`. It bumps the one
   `<Version>` in `Directory.Build.props`, updates `CHANGELOG.md`, commits and tags.
2. Push the commit and the tag. A pushed tag publishes to NuGet and a published version cannot be taken back.

The tag starts two GitHub Actions workflows:

- `publish-nuget.yml` checks the tag against `<Version>`, runs `scripts/pack.sh` (the one list of packages; it fails
  if any package is missing or not in `Fnode.slnx`) and pushes every nupkg to nuget.org through Trusted Publishing.
- `github-release.yml` attaches the same nupkgs to a GitHub Release.

`scripts/publish.fsx` runs `dotnet test --solution`, which needs Microsoft.Testing.Platform; see "Known limits" in
`STATUS.md` for the current state of that preflight step.
