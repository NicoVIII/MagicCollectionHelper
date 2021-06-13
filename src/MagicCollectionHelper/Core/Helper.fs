namespace MagicCollectionHelper.Core

open System

module TryParser =
    // convenient, functional TryParse wrappers returning option<'a>
    let tryParseWith (tryParseFunc: string -> bool * _) =
        tryParseFunc
        >> function
        | true, v -> Some v
        | false, _ -> None

    let parseInt = tryParseWith System.Int32.TryParse
    let parseSingle = tryParseWith System.Single.TryParse
    let parseDouble = tryParseWith System.Double.TryParse
    let parseUint = tryParseWith System.UInt32.TryParse

    // active patterns for try-parsing strings
    let (|Int|_|) = parseInt
    let (|Single|_|) = parseSingle
    let (|Double|_|) = parseDouble
    let (|Uint|_|) = parseUint

module Set =
    let inline ofListList set = set |> Set.ofSeq |> Set.map Set.ofSeq

module Numbers =
    open Dozenalize

    // I would want to use pitman, but it looks like Avalonia has problems with
    // those unicode characters :(
    let config = Config.andrews

    let inline print dozenal precision number =
        if dozenal then
            Display.number config (byte precision) (decimal number)
        else
            String.Format($"{{0:F{precision}}}", number)

module Tuple2 =
    let mapFst f (x, y) = f x, y

    let mapSnd f (x, y) = x, f y

[<RequireQualifiedAccess>]
module Path =
    open System.IO

    let inline combine parts =
        List.fold (fun path part -> Path.Combine(path, part)) "" parts
