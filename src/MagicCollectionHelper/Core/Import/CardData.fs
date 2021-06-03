namespace MagicCollectionHelper.Core.Import

[<RequireQualifiedAccess>]
module CardData =
    open FsHttp
    open FsHttp.DslCE
    open FSharp.Data
    open FSharp.Json
    open Newtonsoft.Json.Linq
    open System
    open System.IO

    open MagicCollectionHelper.Core
    open MagicCollectionHelper.Core.Types
    open MagicCollectionHelper.Core.Types.Generated

    type BulkDataDefaultCardsResponse = { download_uri: string }

    let fetchBulkData (filePath: string) =
        async {
            // Create directory, if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(filePath))
            |> ignore

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

            return filePath
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
                | nr -> nr |> CollectorNumber.fromString |> Some

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

    let prepareImportFile () =
        let filePath =
            [ SystemInfo.savePath
              "default-cards.json" ]
            |> Path.combine

        let fileExists = File.Exists filePath

        let fileOutdated =
            fileExists
            && File.GetCreationTime filePath > (File.GetCreationTime filePath).AddDays 7.

        // If we have a file we use it only for a week
        if not fileExists || fileOutdated then
            fetchBulkData filePath |> DownloadFile
        else
            filePath |> FileExists

    let importFile (filePath: string) =
        async {
            return
                File.ReadLines(filePath)
                |> Seq.map (lineToInfo)
                |> Seq.filter (fun l -> l.IsSome)
                |> Seq.map (fun l -> l.Value)
                |> Seq.fold (fun map info -> Map.add (info.set, info.collectorNumber) info map) Map.empty
        }
