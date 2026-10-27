#!/usr/bin/env bash
# Packs every Fnode package at one version into an output directory (the lockstep release).
#
#   scripts/pack.sh <version> [output-dir]      e.g. scripts/pack.sh 0.2.0 ./artifacts
#
# THE list of packages that make up a release is PACKAGES below. The release workflows
# (.github/workflows/github-release.yml and publish-nuget.yml) call this script and push or attach whatever it
# leaves in the output directory, so adding or removing a package is a one-line change here.
#
# It fails loudly instead of skipping: a listed project that is missing, a pack that fails, or a missing
# <Id>.<version>.nupkg afterwards stops the release. It also refuses a listed project that is not in Fnode.slnx,
# because the solution is what the build and test steps cover, and an untested package must not be published.
set -euo pipefail

# "PackageId:project path"
PACKAGES=(
  "Fnode.Core:src/Fnode.Core/Fnode.Core.fsproj"
  "Fnode.Json:src/Fnode.Json/Fnode.Json.fsproj"
  "Fnode.Yaml:src/Fnode.Yaml/Fnode.Yaml.fsproj"
)

if [ $# -lt 1 ] || [ $# -gt 2 ]; then
  echo "usage: scripts/pack.sh <version> [output-dir]" >&2
  exit 2
fi
version="$1"
output="${2:-./artifacts}"
cd "$(dirname "$0")/.."

for entry in "${PACKAGES[@]}"; do
  id="${entry%%:*}"
  project="${entry#*:}"
  if [ ! -f "$project" ]; then
    echo "::error::$id: project file $project not found" >&2
    exit 1
  fi
  if ! grep -q "$(basename "$project")" Fnode.slnx; then
    echo "::error::$id: $project is not in Fnode.slnx, so it is not built or tested. Add it (and its tests) before releasing." >&2
    exit 1
  fi
done

rm -rf "$output"
mkdir -p "$output"

# -p:Version applies to the whole project graph, so the Fnode.Core dependency that an adapter's nuspec names
# gets the same version as the adapter itself.
for entry in "${PACKAGES[@]}"; do
  project="${entry#*:}"
  dotnet pack "$project" --configuration Release --output "$output" -p:Version="$version"
done

for entry in "${PACKAGES[@]}"; do
  id="${entry%%:*}"
  if [ ! -f "$output/$id.$version.nupkg" ]; then
    echo "::error::$id.$version.nupkg was not produced in $output" >&2
    exit 1
  fi
done

count=$(find "$output" -maxdepth 1 -name '*.nupkg' | wc -l | tr -d ' ')
if [ "$count" -ne "${#PACKAGES[@]}" ]; then
  echo "::error::expected ${#PACKAGES[@]} nupkg files in $output but found $count" >&2
  exit 1
fi
echo "Packed ${#PACKAGES[@]} packages at $version into $output"
