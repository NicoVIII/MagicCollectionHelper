namespace MagicCollectionHelper.Core

[<RequireQualifiedAccess>]
module CardDataImport =
    open Newtonsoft.Json.Linq
    open System.IO

    open MagicCollectionHelper.Core.TryParser
    open MagicCollectionHelper.Core.Types

    let private tokenToColorList (jToken: JToken) =
        match jToken with
        | null -> []
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
            |> Seq.toList

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
                { name = jObject.["name"].ToString()
                  set = jObject.["set"].ToString() |> MagicSet.create
                  collectorNumber = collectorNumber
                  colors = jObject.["colors"] |> tokenToColorList
                  colorIdentity = jObject.["color_identity"] |> tokenToColorList
                  oracleId = jObject.["oracle_id"].ToString() }
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
