open Fake.Core
open Fake.IO
open System
open System.IO
open System.Runtime.InteropServices

module CreateProcess =
    let create cmd args = CreateProcess.fromRawCommand cmd args

type ProcessResult =
    | Ok
    | Error of exitCode: int

module Proc =
    let combine res1 f2 =
        match res1 with
        | Ok -> f2 ()
        | Error x -> Error x

    let run (proc: CreateProcess<ProcessResult<unit>>) =
        printfn $"> %s{proc.CommandLine}"

        Proc.run proc
        |> (fun proc ->
            match proc.ExitCode with
            | 0 -> Ok
            | _ -> Error 2)

let dotnet args =
    CreateProcess.create "dotnet" args |> Proc.run

type JobBuilder() =
    member __.Combine(res1, f2) = Proc.combine res1 f2

    member __.Delay f = f

    member __.For(lst, f) =
        lst
        |> Seq.fold (fun res1 el -> Proc.combine res1 (fun () -> f el)) Ok

    member __.Run f = f ()

    member __.Yield x = x
    member __.Zero() = Ok

let job = JobBuilder()

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
            dotnet [ "tool"; "restore" ]
            dotnet [ "restore"; Config.mainProject ]
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
            printfn "Setup-testdata is only supported on linux!"
            Error 3
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
            [ "-v"; "minimal"
              "-c"; "Release"
              "-o"; Config.packPath
              "/p:SelfContained=true"
              "/p:PublishSingleFile=true"
              "/p:PublishTrimmed=true"
              "/p:TrimMode=Link"
              "/p:IncludeNativeLibrariesForSelfExtract=true"
              "/p:DebugType=None";
              Config.mainProject ]

        Shell.rm Config.packPath
        Shell.mkdir Config.packPath

        job {
            // Linux
            dotnet [ "publish"; "-r"; "linux-x64"; "/p:PublishReadyToRun=true"; yield! commonArgs ]
            Shell.mv $"{Config.packPath}/MagicCollectionHelper.AvaloniaApp" $"{Config.packPath}/MagicCollectionHelper-linux-x64"

            // Windows
            dotnet [ "publish"; "-r"; "win-x64"; yield! commonArgs ]
            Shell.mv $"{Config.packPath}/MagicCollectionHelper.AvaloniaApp.exe" $"{Config.packPath}/MagicCollectionHelper-win-x64.exe"

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
            printfn "Usage: dotnet run [<command>]"
            printfn "Look up available commands in run.fs"
            Error 1
    |> function
        | Ok -> 0
        | Error x -> x
