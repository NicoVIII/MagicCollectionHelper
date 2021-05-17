namespace MagicCollectionHelper.Core

open System
open System.Runtime.InteropServices

/// Holds all relevant paths for the application
[<RequireQualifiedAccess>]
module SystemInfo =
    type OS =
        | Linux
        | MacOS
        | Windows

    /// Holds the OS on which the application is currently running
    let currentOs =
        // Map F# record to OSPlatform
        let mapPlatform platform =
            match platform with
            | Linux -> OSPlatform.Linux
            | MacOS -> OSPlatform.OSX
            | Windows -> OSPlatform.Windows

        // Wrap IsOSPlatform to use with OS record
        let isOS =
            mapPlatform >> RuntimeInformation.IsOSPlatform

        // Determine os
        [ Linux; MacOS; Windows ]
        |> List.tryFind isOS
        |> function
        | Some os -> os
        | None -> failwith "I couldn't determine used OS..."

    /// Path that should be used to save data to
    let savePath =
        match currentOs with
        | Linux
        | MacOS ->
            [ Environment.GetEnvironmentVariable("HOME")
              ".local"
              "share"
              Config.dataFolderName ]
        | Windows ->
            [ Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
              Config.dataFolderName
              "save" ]
        |> Path.combine
