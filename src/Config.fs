namespace MagicCollectionHelper

open Argu
open System

[<RequireQualifiedAccess>]
module Config =
    let programExe = "MagicCollectionHelper(.exe)"
    let missingPercentDefault = 80.0

    let arguErrorHandler =
        ProcessExiter
            (colorizer =
                function
                | ErrorCode.HelpText -> None
                | _ -> Some ConsoleColor.Red)