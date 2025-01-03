namespace MagicCollectionHelper.Core.Import

[<RequireQualifiedAccess>]
module CardData =
    open FsHttp
    open FSharp.Json
    open Newtonsoft.Json.Linq
    open System
    open System.IO

    open MagicCollectionHelper.Core

    type BulkDataDefaultCardsResponse = { download_uri: string }

    let fetchBulkData (filePath: string) =
        async {
            printfn "Fetching bulk data url from scryfall..."

            let! rawResponse =
                http {
                    GET "https://api.scryfall.com/bulk-data/default_cards"
                    Accept "application/json"
                    UserAgent "MagicCollectionHelper"
                }
                |> Request.sendAsync

            let response =
                let text = Response.toText rawResponse

                try
                    Json.deserialize<BulkDataDefaultCardsResponse> text
                with :? JsonDeserializationError as ex ->
                    printfn "Failed to deserialize bulk data response: %s" ex.Message
                    printfn "Response: %s" text
                    failwith "Failed to deserialize bulk data response"

            printfn "Got bulk data download url: %s" response.download_uri

            return! downloadFile response.download_uri filePath
        }

    let private tokenToColorSet (jToken: JToken) =
        jToken.Children()
        |> Seq.map (fun c ->
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

    let private prepareToken fnc token = token |> Option.ofObj |> Option.map fnc

    let private lineToInfo (line: string) : CardInfo option =
        let line' = line.Trim(' ', ',', '[', ']')

        if line' = "" then
            None
        else
            let jObject = JObject.Parse line'

            let typeLine = jObject.["type_line"] |> prepareToken string

            let cmc = jObject.["cmc"] |> prepareToken uint

            let collectorNumber =
                jObject.["collector_number"]
                |> prepareToken string
                |> function
                    | None -> None
                    // There are "dead" entries in the json from scryfall. We don't want them
                    | Some nr when nr.EndsWith "†" -> None
                    | Some "" -> None
                    | Some nr -> nr |> CollectorNumber.fromString |> Some

            match collectorNumber, typeLine, cmc with
            | Some collectorNumber, Some typeLine, Some cmc ->
                {
                    name = jObject.["name"] |> string
                    set = jObject.["set"] |> string |> MagicSet.create
                    collectorNumber = collectorNumber
                    colors =
                        Option.ofObj jObject.["colors"]
                        |> function
                            | Some token -> tokenToColorSet token
                            | None -> Set.empty
                    colorIdentity = jObject.["color_identity"] |> tokenToColorSet
                    oracleId = jObject.["oracle_id"] |> string
                    rarity =
                        jObject.["rarity"]
                        |> Option.ofObj
                        |> Option.map (string >> Option.ofObj)
                        |> Option.flatten
                        |> function
                            | Some value ->
                                value
                                |> (fun s -> s.Trim())
                                // Capitalize
                                |> Seq.mapi (fun i c ->
                                    match i with
                                    | 0 -> Char.ToUpper(c)
                                    | _ -> c)
                                |> String.Concat
                                |> Rarity.fromString
                                |> function
                                    | Some rarity -> rarity
                                    | None -> failwith "We have no valid rarity for now.."
                            | None -> failwith "Got no rarity for some reason.."
                    typeLine = typeLine
                    cmc = cmc
                }
                |> Some
            | _ -> None

    let prepareImportFile () =
        let filePath = [ SystemInfo.savePath; "default-cards.json" ] |> Path.combine

        let fileExists = File.Exists filePath

        let fileOutdated =
            fileExists
            && DateTime.Now > (File.GetCreationTime filePath).AddHours Config.maxAgeCardDataHours

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
