namespace MagicCollectionHelper

open Argu
open System
open System.IO

open MagicCollectionHelper.InputValidation
open MagicCollectionHelper.Types

/// This module handles the command line call and prepares the arguments for
/// the analyzer
module Program =
    /// Prepares command line arguments
    let handleArguments argv =
        // Use Argu to parse the arguments
        let parser =
            ArgumentParser.Create<CliArguments>(programName = Config.programExe, errorHandler = Config.arguErrorHandler)

        let results = parser.ParseCommandLine argv

        // Validate all parameters
        let filePath =
            results.GetResult CollectionFile
            |> validateFilePath

        let missingPercent =
            results.TryGetResult MissingPercent
            |> validateMissingPercent

        // Use Applicatives to construct the result of the validation
        ProgramConfig.create
        <!> filePath
        <*> missingPercent

    [<EntryPoint>]
    let main argv =
        // Check, if arguments are valid
        match handleArguments argv with
        | Ok arguments ->
            let setData = CardData.createSetData ()
            Analyser.analyse setData arguments
            |> Seq.iter (printfn "%s")

            0
        | Error errors -> handleErrors errors
