# Fnode.Yaml

The YAML adapter for [Fnode.Core](https://www.nuget.org/packages/Fnode.Core): parses YAML text to a `Node` with
source locations, the original timestamp text and the original number text, and prints a `Node` back as YAML.

`parsed.ScalarText` maps the pointer of each number to the text it was written as (`1.10`, `1.0`, `1e3`), which
`Node` cannot keep (`1.10` is `Float 1.1`). A number reached through an alias reports the anchor's text. For
duplicate keys, `Node` keeps every pair, and `Locations` and `ScalarText` describe the first occurrence. A non-string
mapping key is a `type.mismatch` error that carries the location of the mapping.

Released in lockstep with `Fnode.Core` (same version, one tag). It needs only `Fnode.Core` and `Flexi.Yaml`, and
works without `Fnode.Json`.
