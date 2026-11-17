namespace Fnode

/// 1-based line and column in the source text.
type Pos = { Line: int; Column: int }

type Span = { Start: Pos; End: Pos }

/// A parsed document plus the source spans of its nodes, keyed by printed JSON Pointer.
///
/// - `Locations`: the source span of each node.
/// - `ScalarText`: the text each `Int`/`Float` node was written as in the source (`1.10`, `1.0`, `1e3`,
///   or an integer beyond int64), which `Node` cannot keep. A pointer with no entry has no recoverable text
///   (an adapter that does not provide it, or a number reached through a YAML alias); use the `Node` value.
/// - For duplicate object keys, `Node` keeps both pairs (lookups take the first) but `Locations` and
///   `ScalarText` describe the first occurrence only.
type Parsed =
    { Node: Node
      Locations: Map<string, Span>
      ScalarText: Map<string, string> }

/// Structural failure (parse, path, type).
type Error =
    { Code: string
      Message: string
      Path: Path
      Location: Span option
      /// Structured context as ordered key/value pairs (for example `op`, `rule`, `input`, `file`).
      /// Keys are defined by the producer; the core never interprets them.
      Context: (string * string) list }

/// Stable error codes.
module ErrorCode =
    [<Literal>]
    let parse = "parse"
    [<Literal>]
    let pathNotFound = "path.notFound"
    [<Literal>]
    let typeMismatch = "type.mismatch"
    [<Literal>]
    let pathInvalid = "path.invalid"

module Error =
    let create (code: string) (message: string) (path: Path) : Error =
        { Code = code; Message = message; Path = path; Location = None; Context = [] }

    /// Add a context entry. Later entries are outer context: `describe` prints them first.
    let withContext (key: string) (value: string) (e: Error) : Error =
        { e with Context = e.Context @ [ (key, value) ] }

    let mapMessage (f: string -> string) (e: Error) : Error = { e with Message = f e.Message }

    /// Human-readable text: outermost context first, then the message,
    /// for example `rule 2: input 'team': message`.
    let describe (e: Error) : string =
        let prefix =
            e.Context
            |> List.rev
            |> List.map (fun (k, v) -> k + " " + v + ": ")
            |> String.concat ""
        prefix + e.Message
