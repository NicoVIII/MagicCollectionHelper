namespace MagicCollectionHelper

module TryParser =
    // convenient, functional TryParse wrappers returning option<'a>
    let tryParseWith (tryParseFunc: string -> bool * _) =
        tryParseFunc
        >> function
        | true, v -> Some v
        | false, _ -> None

    let parseDate = tryParseWith System.DateTime.TryParse
    let parseInt = tryParseWith System.Int32.TryParse
    let parseSingle = tryParseWith System.Single.TryParse
    let parseDouble = tryParseWith System.Double.TryParse
    let parseUint = tryParseWith System.UInt32.TryParse

    // active patterns for try-parsing strings
    let (|Date|_|) = parseDate
    let (|Int|_|) = parseInt
    let (|Single|_|) = parseSingle
    let (|Double|_|) = parseDouble
    let (|Uint|_|) = parseUint
