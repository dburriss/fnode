# Tutorial: read and change a YAML document

In this tutorial you build a small F# program that parses YAML, reads a value, finds where it was written, changes
it, prints the result and handles two kinds of failure. It takes about ten minutes and needs the .NET SDK 8 or
newer.

## 1. Create the project

```sh
dotnet new console -lang "F#" -o FnodeTour
cd FnodeTour
dotnet add package Fnode.Yaml --prerelease
```

## 2. Parse some YAML

Replace the contents of `Program.fs` with:

```fsharp
open Fnode

let text = "name: web\nreplicas: 2\nports: [80, 443]\n"

let parsed =
    match Yaml.format.Parse text with
    | Ok p -> p
    | Error errors -> failwith (errors |> List.map Error.describe |> String.concat "; ")
```

`Yaml.format.Parse` returns a `Parsed`: the document as a `Node` tree (`parsed.Node`) plus where each part was
written (`parsed.Locations`).

## 3. Read a value

Add a helper that turns a JSON Pointer such as `/ports/1` into a `Path`, then read that path:

```fsharp
let pointer text =
    match Path.parse text with
    | Ok p -> p
    | Error e -> failwith (Error.describe e)

printfn "%A" (Node.tryGet (pointer "/ports/1") parsed.Node)
```

## 4. Find where it was written

```fsharp
let span = parsed.Locations.["/ports/1"]
printfn "line %d, column %d" span.Start.Line span.Start.Column
```

## 5. Change a value and print it

```fsharp
match Node.set (pointer "/replicas") (Int 3L) parsed.Node with
| Ok edited -> printf "%s" (Yaml.format.Print edited)
| Error e -> printfn "%s" (Error.describe e)
```

## 6. See a failure

Ask for a path that does not exist, then parse broken YAML:

```fsharp
match Node.tryGet (pointer "/image/tag") parsed.Node with
| Ok _ -> ()
| Error e -> printfn "%s at %s" e.Code (Path.print e.Path)

match Yaml.format.Parse "a: [1, 2\nb: 3" with
| Error (e :: _) -> printfn "%s (line %d)" e.Message e.Location.Value.Start.Line
| _ -> ()
```

## 7. Run it

```sh
dotnet run
```

You should see:

```
Ok (Int 443L)
line 3, column 13
name: web
replicas: 3
ports:
  - 80
  - 443
path.notFound at /image
Expected ',' or ']' in flow sequence (line 2)
```

You parsed YAML to a `Node`, navigated it with a JSON Pointer, edited it without mutating anything, and got every
failure back as an `Error` value with a code, a path and, for parse errors, a source location.

## Next

- Look up a function or type in the [reference](reference.md).
- Read why `Node` is the working form in [`ARCHITECTURE.md`](../ARCHITECTURE.md).
