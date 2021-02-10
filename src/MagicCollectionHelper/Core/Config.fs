namespace MagicCollectionHelper.Core

open Argu
open System

[<RequireQualifiedAccess>]
module Config =
    let programExe = "MagicCollectionHelper(.exe)"
    let missingPercentDefault = 0.8

    let arguErrorHandler =
        ProcessExiter(
            colorizer =
                function
                | ErrorCode.HelpText -> None
                | _ -> Some ConsoleColor.Red
        )
