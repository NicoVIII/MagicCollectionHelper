namespace MagicCollectionHelper.Core

[<RequireQualifiedAccess>]
module CardDataImport =
    open Newtonsoft.Json.Linq
    open System
    open System.IO

    open MagicCollectionHelper.Core.TryParser
    open MagicCollectionHelper.Core.Types
    open MagicCollectionHelper.Core.Types.Generated

    let private tokenToColorSet (jToken: JToken) =
        match jToken with
        | null -> Set.empty
        | jToken ->
            jToken.Children()
            |> Seq.map
                (fun c ->
                    c.ToString()
                    |> function
                    | "W" -> White
                    | "U" -> Blue
                    | "B" -> Black
                    | "R" -> Red
                    | "G" -> Green
                    | c -> failwith $"Unknown color string: {c}")
            |> Set.ofSeq

    let private lineToInfo (line: string) : CardInfo option =
        let line' = line.Trim(' ', ',', '[', ']')

        if line' = "" then
            None
        else
            let jObject = JObject.Parse line'

            let collectorNumber =
                jObject.["collector_number"].ToString()
                |> function
                | Uint number -> number |> CollectorNumber |> Some
                | _ -> None

            match collectorNumber with
            | Some collectorNumber ->
                { name = jObject.["name"] |> string
                  set = jObject.["set"] |> string |> MagicSet.create
                  collectorNumber = collectorNumber
                  colors = jObject.["colors"] |> tokenToColorSet
                  colorIdentity = jObject.["color_identity"] |> tokenToColorSet
                  oracleId = jObject.["oracle_id"] |> string
                  rarity =
                      jObject.["rarity"]
                      |> string
                      |> (Seq.mapi
                              (fun i c ->
                                  match i with
                                  | 0 -> Char.ToUpper(c)
                                  | _ -> c)
                          >> String.Concat)
                      |> Rarity.fromString
                      |> Option.get }
                |> Some
            | None -> None

    let private parseJson (filePath: string) =
        File.ReadLines(filePath)
        |> Seq.map (lineToInfo)
        |> Seq.filter (fun l -> l.IsSome)
        |> Seq.map (fun l -> l.Value)
        |> Seq.fold (fun map info -> Map.add (info.set, info.collectorNumber) info map) Map.empty

    let private searchImportFile () =
        Directory.EnumerateFiles "."
        |> Seq.sortDescending
        |> Seq.tryFind (
            Path.GetFileName
            >> (fun (s: string) -> s.StartsWith "default-cards")
        )
        // Convert to absolute path
        |> Option.map (Path.GetFullPath)

    let perform =
        searchImportFile >> Option.map (parseJson)

    // Because import could become an expensive task, we provide an async version
    let performAsync () = async { return perform () }
