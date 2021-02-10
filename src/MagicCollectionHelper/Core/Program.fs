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
    let handleArguments argv: ProgramConfig =
        // Use Argu to parse the arguments
        let parser =
            ArgumentParser.Create<CliArguments>(programName = Config.programExe, errorHandler = Config.arguErrorHandler)

        let results = parser.ParseCommandLine argv

        // Validate all parameters
        let dozenalize = results.Contains Dozenalize

        let filePath =
            results.PostProcessResult(<@ CollectionFile @>, validateFilePath)

        let missingPercent =
            if results.Contains MissingPercent then
                results.PostProcessResult(<@ MissingPercent @>, validateMissingPercent)
            else
                Config.missingPercentDefault

        let setWithFoils = results.Contains SetWithFoils

        // Use Applicatives to construct the result of the validation
        { dozenalize = dozenalize
          filePath = filePath
          missingPercent = missingPercent
          setWithFoils = setWithFoils }

    [<EntryPoint>]
    let main argv =
        // Check, if arguments are valid
        let config = handleArguments argv
        let setData = CardData.createSetData ()

        Analyser.analyse setData config
        |> Seq.iter (printfn "%s")

        0
