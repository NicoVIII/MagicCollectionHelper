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
    // I would want to use pitman, but it looks like Avalonia has problems with
    // those unicode characters :(
    let dozenalConfig = Dozenalize.Config.andrews

    let inline percent basePref number =
        let one = LanguagePrimitives.GenericOne

        let factor =
            match basePref with
            | Dozenal -> 144
            | Seximal -> 36
            | Decimal -> 100

        let genericFactor = (Seq.init factor (fun _ -> one)) |> Seq.sum

        number * genericFactor

    let inline print basePref precision number =
        match basePref with
        | Dozenal -> Dozenalize.Display.number dozenalConfig (byte precision) (decimal number)
        | Seximal -> Seximalize.Display.number (byte precision) (decimal number)
        | Decimal -> String.Format($"{{0:F{precision}}}", number)

module Tuple2 =
    let mapFst f (x, y) = f x, y

    let mapSnd f (x, y) = x, f y

[<RequireQualifiedAccess>]
module Path =
    open System.IO

    let inline combine parts =
        List.fold (fun path part -> Path.Combine(path, part)) "" parts
