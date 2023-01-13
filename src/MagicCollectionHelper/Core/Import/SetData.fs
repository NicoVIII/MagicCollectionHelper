namespace MagicCollectionHelper.Core.Import

open FSharp.Data
open FSharp.Json
open System.IO

open MagicCollectionHelper.Core

module SetData =
    type SetDataCSV = {
        code: string
        name: string
        released_at: string
        card_count: uint
    }

    type SetDataListCSV = { data: SetDataCSV list }

    let fetchSetData (filePath: string) = async { return! downloadFile "https://api.scryfall.com/sets" filePath }

    let prepareImportFile () =
        let filePath = [ SystemInfo.savePath; "setdata.json" ] |> Path.combine

        let fileExists = File.Exists filePath

        let fileOutdated =
            fileExists
            && File.GetCreationTime filePath > (File.GetCreationTime filePath).AddHours Config.maxAgeCardDataHours

        // If we have a file we use it only for a week
        if not fileExists || fileOutdated then
            fetchSetData filePath |> DownloadFile
        else
            filePath |> FileExists

    let importFile filePath = async {
        let! text = File.ReadAllTextAsync(filePath) |> Async.AwaitTask

        return
            text
            |> Json.deserialize<SetDataListCSV>
            |> (fun data -> data.data)
            |> List.map (fun setData ->
                let setCode = setData.code.ToUpper() |> MagicSet.create

                let setData = {
                    SetData.name = setData.name
                    date = setData.released_at
                    max = setData.card_count
                }

                setCode, setData)
            |> Map.ofList
    }
