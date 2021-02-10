#!/bin/dotnet fsi
#r "nuget: Fake.DotNet.Cli"

open Fake.DotNet
open System.IO

/// Contains config options for bundling script
module Config =
    let outputPath = "./deploy"

    let projectPath =
        "./src/MagicCollectionHelper/AvaloniaApp/MagicCollectionHelper.AvaloniaApp.fsproj"

/// Supported Runtimes, which can be created with this script
type Runtime =
    | Linux
    | MacOS
    | Windows

module Runtime =
    /// Returns the dotnet identifier for this runtime
    let toValue runtime =
        match runtime with
        | Linux -> "linux-x64"
        | MacOS -> "osx-x64"
        | Windows -> "win-x64"

    // Returns runtime specific properties for the build process
    let getProperties runtime =
        match runtime with
        | Linux -> [ "PublishReadyToRun", "true" ]
        | MacOS
        | Windows -> []

    /// Return the file extension for a specific runtime
    let getFileExtension runtime =
        match runtime with
        | Linux
        | MacOS -> ""
        | Windows -> ".exe"

/// Publish the project for a specific runtime
let publish runtime =
    let rtExt = Runtime.getFileExtension runtime
    let rtValue = Runtime.toValue runtime

    let options (options: DotNet.PublishOptions) =
        let msBuildProperties =
            [ "PublishSingleFile", "true"
              "PublishTrimmed", "true"
              "TrimMode", "Link"
              "IncludeNativeLibrariesForSelfExtract", "true"
              "DebugType", "None" ]
            |> List.append (Runtime.getProperties runtime)
            |> List.append options.MSBuildParams.Properties

        { options with
              Common =
                  { options.Common with
                        Verbosity = Some DotNet.Verbosity.Minimal }
              Configuration = DotNet.BuildConfiguration.Release
              MSBuildParams =
                  { options.MSBuildParams with
                        Properties = msBuildProperties }
              OutputPath = Some Config.outputPath
              Runtime = Some rtValue
              SelfContained = Some true }

    // Publish with DotNet
    DotNet.publish options Config.projectPath

    // Rename file
    ($"MagicCollectionHelper.AvaloniaApp{rtExt}", $"MagicCollectionHelper-{rtValue}{rtExt}")
    |> (fun (a, b) ->
        // Add output path to start of file path
        let addOutputPath s = Path.Combine(Config.outputPath, s)
        (addOutputPath a, addOutputPath b))
    |> File.Move

// Start of script
Directory.Delete(Config.outputPath, true)

publish Linux

publish Windows
// macOS single-file publishing is not working, there are dylibs generated while publishing for some reason
//publish MacOS
