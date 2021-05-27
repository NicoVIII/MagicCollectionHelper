namespace MagicCollectionHelper.Core.Import

open FSharp.Data
open FSharp.Json
open System.IO

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

module SetData =
    type SetDataCSV =
        { code: string
          name: string
          released_at: string
          card_count: uint }

    type SetDataListCSV = { data: SetDataCSV list }

    let fetchSetData (filePath: string) =
        async {
            // Create directory, if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(filePath))
            |> ignore

            let! fileRequest = Http.AsyncRequestStream("https://api.scryfall.com/sets")

            use outputFile =
                new FileStream(filePath, FileMode.Create)

            do!
                fileRequest.ResponseStream.CopyToAsync(outputFile)
                |> Async.AwaitTask

            return filePath
        }

    let prepareImportFile () =
        let filePath =
            [ SystemInfo.savePath; "setdata.json" ]
            |> Path.combine

        let fileExists = File.Exists filePath

        let fileOutdated =
            fileExists
            && File.GetCreationTime filePath > (File.GetCreationTime filePath).AddDays 7.

        // If we have a file we use it only for a week
        if not fileExists || fileOutdated then
            fetchSetData filePath |> DownloadFile
        else
            filePath |> FileExists

    let importFile filePath =
        async {
            let! text = File.ReadAllTextAsync(filePath) |> Async.AwaitTask

            return
                text
                |> Json.deserialize<SetDataListCSV>
                |> (fun data -> data.data)
                |> List.map
                    (fun setData ->
                        let setCode =
                            setData.code.ToUpper() |> MagicSet.create

                        let setData =
                            { SetData.name = setData.name
                              date = setData.released_at
                              max = setData.card_count }

                        setCode, setData)
                |> Map.ofList
        }
