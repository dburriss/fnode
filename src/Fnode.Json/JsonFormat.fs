namespace Fnode

open Flexi.Json

/// The JSON `Format`.
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Json =

    let private pos (p: Flexi.Json.Pos) : Fnode.Pos = { Line = p.Line; Column = p.Column }

    let private span (s: Flexi.Json.Span) : Fnode.Span = { Start = pos s.Start; End = pos s.End }

    let private parse (text: string) : Result<Parsed, Error list> =
        try
            match JsonValue.TryParseWithMetadata text with
            | Ok parsed ->
                Ok
                    { Node = JsonConvert.toNode parsed.Root
                      Locations = parsed.Spans |> Map.map (fun _ s -> span s)
                      ScalarText = parsed.NumberText }
            | Error e ->
                let p = pos e.Pos
                Error [ { Error.create ErrorCode.parse e.Message [] with
                            Location = Some { Start = p; End = p } } ]
        with ex ->
            Error [ Error.create ErrorCode.parse ex.Message [] ]

    let private print (node: Node) : string =
        (JsonConvert.ofNode node).ToString(true)

    /// JSON format: parse with locations, pretty (2-space) print.
    let format: Format =
        { Name = "json"
          Extensions = [ ".json" ]
          Parse = parse
          Print = print }
