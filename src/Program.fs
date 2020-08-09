namespace MagicCollectionHelper

open MagicCollectionHelper.ErrorHandling
open MagicCollectionHelper.Types

open System.IO

/// This module handles the command line call and prepares the arguments for
/// the analyzer
module Program =
    /// Prepares command line arguments
    let handleArguments argv =
        // Atm we just accept a filepath for the csv file
        match argv with
        | [| filePath |] ->
            let filePath = Path.GetFullPath filePath

            if File.Exists filePath then Ok { filePath = filePath } else Error NonExistingFile
        | _ -> Error InvalidArguments

    [<EntryPoint>]
    let main argv =
        // Check, if arguments are valid
        match handleArguments argv with
        | Ok arguments ->
            Analyzer.analyze arguments.filePath
            |> printfn "%A"
            0
        | Error error -> handleError error
