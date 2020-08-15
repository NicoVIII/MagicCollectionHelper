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
            let (basicResult, setResult), languageResult = Analyzer.analyze arguments.filePath
            printfn "%A" basicResult
            setResult
            |> Map.iter (fun (MagicSet key) value -> printfn "%s - %i: %A" key (snd value) (fst value))
            languageResult
            |> Map.iter (fun (Language key) value -> printfn "%s: %i" key value)
            0
        | Error error -> handleError error
