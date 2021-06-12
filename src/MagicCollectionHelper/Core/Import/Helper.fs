namespace MagicCollectionHelper.Core.Import

open FSharp.Data
open System.IO

[<AutoOpen>]
module Helper =
    /// Downloads file from given url and returns path after that
    let downloadFile url (path: string) =
        async {
            // Create directory, if it doesn't exist
            Path.GetDirectoryName path
            |> Directory.CreateDirectory
            |> ignore

            let! fileRequest = Http.AsyncRequestStream(url)

            use outputFile = new FileStream(path, FileMode.Create)

            do!
                fileRequest.ResponseStream.CopyToAsync(outputFile)
                |> Async.AwaitTask

            return path
        }
