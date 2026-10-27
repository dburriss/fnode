# Fnode.Json

The JSON adapter for [Fnode.Core](https://www.nuget.org/packages/Fnode.Core): parses JSON text to a `Node` with
source locations (line and column per JSON Pointer) and prints a `Node` back as pretty JSON. Built on
[Flexi.Json](https://www.nuget.org/packages/Flexi.Json) (strict RFC 8259). Runs on .NET, Fable JavaScript and
Fable Python; the package ships its F# sources under `fable/`.

Released in lockstep with `Fnode.Core` (same version, one tag). It needs only `Fnode.Core` and `Flexi.Json`, and
works without `Fnode.Yaml`.

## Install

```sh
dotnet add package Fnode.Json --prerelease
```

## Example

```fsharp
open Fnode

match Json.format.Parse """{ "name": "web", "ports": [80, 443] }""" with
| Ok parsed -> parsed.Node, parsed.Locations.["/ports/1"]   // Obj [...], span of 443
| Error errors -> failwithf "%A" errors

Json.format.Print(Obj [ "a", Int 1L ])   // pretty (2-space) JSON
```

`Timestamp` prints as a JSON string and JSON strings parse back as `Str`. Parse failures come back as an
`Error list` with the line and column of the problem.
