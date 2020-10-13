namespace MagicCollectionHelper

open System.IO

module InputValidation =
    type Error =
        | NonExistingFile
        | InvalidMissingPercent

    /// Returns an error message to the console for a specific error
    let errorToString error =
        match error with
        | NonExistingFile -> "Filepath invalid, file does not exist"
        | InvalidMissingPercent -> "Percent has to be between 0 and 100"

    /// Prints error messages to the console for a list of errors and returns
    /// a non-zero error message code
    let handleErrors errors =
        errors
        |> List.map errorToString
        |> List.iter (printfn "%s")
        1 // Return non-zero exit code

    let validateFilePath filePath =
        let filePath = filePath |> Path.GetFullPath
        if File.Exists filePath then Ok filePath else Error [ NonExistingFile ]

    let validateMissingPercent missingPercent =
        let missingPercent =
            missingPercent
            |> Option.defaultValue Config.missingPercentDefault

        match missingPercent with
        | mp when mp >= 0.0 && mp <= 100.0 -> Ok mp
        | _ -> Error [ InvalidMissingPercent ]
