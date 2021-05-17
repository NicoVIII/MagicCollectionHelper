namespace MagicCollectionHelper.Core

open System

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

module Result =
    let apply fResult xResult =
        match fResult, xResult with
        | Ok f, Ok x -> Ok(f x)
        | Error ex, Ok _ -> Error ex
        | Ok _, Error ex -> Error ex
        | Error ex1, Error ex2 -> Error(List.concat [ ex1; ex2 ])

[<AutoOpen>]
module ResultInfix =
    // Define result infix operators
    let (<!>) = Result.map
    let (<*>) = Result.apply

open Dozenalize

module Set =
    let inline ofListList set = set |> Set.ofSeq |> Set.map Set.ofSeq

module Numbers =
    let config = Types.Config.PreConf.pitman

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

[<RequireQualifiedAccess>]
module File =
    open System.IO

    let inline readAllText filePath =
        if File.Exists filePath then
            async {
                let! text = File.ReadAllTextAsync(filePath) |> Async.AwaitTask
                return text |> Some
            }
        else
            None |> async.Return

    let inline writeAllText (filePath: string) text =
        let folder = Path.GetDirectoryName filePath
        // Create folder, if it does not exist
        if not (Directory.Exists folder) then
            Directory.CreateDirectory folder |> ignore

        async {
            do!
                File.WriteAllTextAsync(filePath, text)
                |> Async.AwaitTask
        }
