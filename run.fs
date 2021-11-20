open System
open System.IO
open System.Runtime.InteropServices

open Fake.IO

open RunHelpers
open RunHelpers.BasicShortcuts

[<RequireQualifiedAccess>]
module Config =
    let dataFolder = "./data"

    let mainProject =
        "./src/MagicCollectionHelper/AvaloniaApp/MagicCollectionHelper.AvaloniaApp.fsproj"

    let testFolder = "./tests"

    let packPath = "./deploy"

module Task =
    let restore () =
        job {
            Template.DotNet.toolRestore ()
            Template.DotNet.restore Config.mainProject
        }

    let build () =
        dotnet [
            "build"
            Config.mainProject
            "--no-restore"
        ]

    let run () =
        dotnet [
            "run"
            "--project"
            Config.mainProject
        ]

    let test () =
        let projects =
            Directory.EnumerateFiles(Config.testFolder, "*.fsproj", SearchOption.AllDirectories)

        job {
            for project in projects do
                printfn "\nRun tests in %s:" project

                dotnet [ "run"; "--project"; project ]
        }

    let setupTestdata () =
        if not (RuntimeInformation.IsOSPlatform OSPlatform.Linux) then
            Error(3, [ "Setup-testdata is only supported on linux!" ])
        else
            // Move workspace files
            Path.Combine(Config.dataFolder, "workspace")
            |> Directory.EnumerateFiles
            |> Shell.copy "."

            // Move share files
            Path.Combine(Config.dataFolder, "share")
            |> Directory.EnumerateFiles
            |> Shell.copy (
                Path.Combine(
                    Environment.GetEnvironmentVariable("HOME"),
                    ".local",
                    "share",
                    "magic-collection-helper"
                )
            )

            Ok

    let publish () =
        let commonArgs =
            [ "-v"
              "minimal"
              "-c"
              "Release"
              "-o"
              Config.packPath
              "/p:SelfContained=true"
              "/p:PublishSingleFile=true"
              "/p:PublishTrimmed=true"
              "/p:TrimMode=Link"
              "/p:IncludeNativeLibrariesForSelfExtract=true"
              "/p:DebugType=None"
              Config.mainProject ]

        Shell.rm Config.packPath
        Shell.mkdir Config.packPath

        job {
            // Linux
            dotnet [
                "publish"
                "-r"
                "linux-x64"
                "/p:PublishReadyToRun=true"
                yield! commonArgs
            ]

            Shell.mv
                $"{Config.packPath}/MagicCollectionHelper.AvaloniaApp"
                $"{Config.packPath}/MagicCollectionHelper-linux-x64"

            // Windows
            dotnet [
                "publish"
                "-r"
                "win-x64"
                yield! commonArgs
            ]

            Shell.mv
                $"{Config.packPath}/MagicCollectionHelper.AvaloniaApp.exe"
                $"{Config.packPath}/MagicCollectionHelper-win-x64.exe"

        // macOS single-file publishing is not working, there are dylibs generated while publishing for some reason
        //dotnet [ "publish"; "-r"; "osx-x64"; yield! commonArgs ]
        }

module Command =
    let restore () = Task.restore ()

    let build () =
        job {
            restore ()
            Task.build ()
        }

    let run () =
        job {
            restore ()
            Task.run ()
        }

    let test () =
        job {
            restore ()
            Task.test ()
        }

    let publish () =
        job {
            restore ()
            Task.publish ()
        }

    let setupTestdata () = Task.setupTestdata ()

[<EntryPoint>]
let main args =
    args
    |> List.ofArray
    |> function
        | [ "restore" ] -> Command.restore ()
        | [ "build" ] -> Command.build ()
        | []
        | [ "run" ] -> Command.run ()
        | [ "test" ] -> Command.test ()
        | [ "publish" ] -> Command.publish ()
        | [ "setup-testdata" ] -> Command.setupTestdata ()
        | _ ->
            let msg =
                [ "Usage: dotnet run [<command>]"
                  "Look up available commands in run.fs" ]

            Error(1, msg)
    |> ProcessResult.wrapUp
