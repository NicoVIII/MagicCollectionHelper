namespace MagicCollectionHelper.Core

[<RequireQualifiedAccess>]
module CardDataImport =
    open FsHttp
    open FsHttp.DslCE
    open FSharp.Data
    open FSharp.Json
    open Newtonsoft.Json.Linq
    open System
    open System.IO

    open MagicCollectionHelper.Core.TryParser
    open MagicCollectionHelper.Core.Types
    open MagicCollectionHelper.Core.Types.Generated

    type BulkDataDefaultCardsResponse = { download_uri: string }

    let fetchBulkData filePath =
        async {
            let! rawResponse = httpAsync { GET "https://api.scryfall.com/bulk-data/default_cards" }

            let response =
                Response.toText rawResponse
                |> Json.deserialize<BulkDataDefaultCardsResponse>

            let! fileRequest = Http.AsyncRequestStream(response.download_uri)

            use outputFile =
                new FileStream(filePath, FileMode.Create)

            do!
                fileRequest.ResponseStream.CopyToAsync(outputFile)
                |> Async.AwaitTask
        }

    let private tokenToColorSet (jToken: JToken) =
        match jToken with
        | null -> Set.empty
        | jToken ->
            jToken.Children()
            |> Seq.map
                (fun c ->
                    c
                    |> string
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
                jObject.["collector_number"]
                |> string
                |> function
                // There are "dead" entries in the json from scryfall. We don't want them
                | nr when nr.EndsWith "†" -> None
                | "" -> None
                | nr -> nr |> CollectorNumber |> Some

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
                      |> function
                      | Some rarity -> rarity
                      | None ->
                          let rarity = jObject.["rarity"] |> string
                          failwith $"{rarity} is no valid rarity for now..."
                  typeLine = jObject.["type_line"] |> string
                  cmc = jObject.["cmc"] |> uint }
                |> Some
            | None -> None

    let private parseJson (filePath: string) =
        File.ReadLines(filePath)
        |> Seq.map (lineToInfo)
        |> Seq.filter (fun l -> l.IsSome)
        |> Seq.map (fun l -> l.Value)
        |> Seq.fold (fun map info -> Map.add (info.set, info.collectorNumber) info map) Map.empty

    let private getImportFile () =
        async {
            let path =
                [ SystemInfo.savePath
                  "default-cards.json" ]
                |> Path.combine

            // If we have a file we use it only for a week
            if not (File.Exists path)
               || File.GetCreationTime path > (File.GetCreationTime path).AddDays 7. then
                printfn "Download default card data..."
                do! fetchBulkData path

            return path
        }

    let perform () =
        async {
            let! filePath = getImportFile ()
            return parseJson filePath
        }
