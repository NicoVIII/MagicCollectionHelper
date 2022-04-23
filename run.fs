open System
open System.IO
open System.Runtime.InteropServices

open Fake.IO

open RunHelpers
open RunHelpers.Templates

[<RequireQualifiedAccess>]
module Config =
    let dataFolder = "./data"

    let mainProject =
        "./src/MagicCollectionHelper/AvaloniaApp/MagicCollectionHelper.AvaloniaApp.fsproj"

    let testFolder = "./tests"

    let packPath = "./deploy"

module Task =
    let restore () =
        DotNet.restoreWithTools Config.mainProject

    let build () = DotNet.build Config.mainProject Debug
    let run () = DotNet.run Config.mainProject

    let test () =
        let projects =
            Directory.EnumerateFiles(Config.testFolder, "*.fsproj", SearchOption.AllDirectories)

        job {
            for project in projects do
                printfn "\nRun tests in %s:" project

                DotNet.run project
        }

    let setupTestdata () =
        if not (RuntimeInformation.IsOSPlatform OSPlatform.Linux) then
            Job.error 3 [ "Setup-testdata is only supported on linux!" ]
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

            Job.ok

    let publish () =
        Shell.rm Config.packPath
        Shell.mkdir Config.packPath

        job {
            DotNet.publishSelfContained Config.packPath Config.mainProject LinuxX64

            Shell.mv
                $"{Config.packPath}/MagicCollectionHelper.AvaloniaApp"
                $"{Config.packPath}/MagicCollectionHelper-linux-x64"

            // Windows
            DotNet.publishSelfContained Config.packPath Config.mainProject WindowsX64

            Shell.mv
                $"{Config.packPath}/MagicCollectionHelper.AvaloniaApp.exe"
                $"{Config.packPath}/MagicCollectionHelper-win-x64.exe"
        }

[<EntryPoint>]
let main args =
    args
    |> List.ofArray
    |> function
        | [ "restore" ] -> Task.restore ()
        | [ "build" ] ->
            job {
                Task.restore ()
                Task.build ()
            }
        | []
        | [ "run" ] ->
            job {
                Task.restore ()
                Task.run ()
            }
        | [ "test" ] ->
            job {
                Task.restore ()
                Task.test ()
            }
        | [ "publish" ] ->
            job {
                Task.restore ()
                Task.publish ()
            }
        | [ "setup-testdata" ] -> Task.setupTestdata ()
        | _ ->
            Job.error 1 [ "Usage: dotnet run [<command>]"
                          "Look up available commands in run.fs" ]
    |> Job.execute
