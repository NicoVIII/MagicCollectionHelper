namespace MagicCollectionHelper.Core.Import

open FSharp.Data
open System.IO

open MagicCollectionHelper.Core

[<AutoOpen>]
module Helper =
    /// Downloads file from given url and returns path after that
    let downloadFile url (path: string) =
        async {
            // Create directory, if it doesn't exist
            Path.GetDirectoryName path |> Directory.CreateDirectory |> ignore

            printfn "Download file from %s to %s" url path

            let! fileRequest =
                Http.AsyncRequestStream(url, headers = [ "Accept", "application/json"; "User-Agent", Config.userAgent ])

            use outputFile = new FileStream(path, FileMode.Create)

            do! fileRequest.ResponseStream.CopyToAsync(outputFile) |> Async.AwaitTask

            printfn "Finished download to %s" path

            return path
        }
