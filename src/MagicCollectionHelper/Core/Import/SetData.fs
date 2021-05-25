namespace MagicCollectionHelper.Core.Import

open FsHttp
open FsHttp.DslCE
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

    let private fetchSetData (filePath: string) =
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
        }

    let private getImportFile () =
        async {
            let path =
                [ SystemInfo.savePath; "setdata.json" ]
                |> Path.combine

            // If we have a file we use it only for a week
            if not (File.Exists path)
               || File.GetCreationTime path > (File.GetCreationTime path).AddDays 7. then
                printfn "Download set data..."
                do! fetchSetData path

            return path
        }

    let private parseJson filePath =
        File.ReadAllText(filePath)
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

    let import () =
        async {
            let! filePath = getImportFile ()
            return parseJson filePath
        }
