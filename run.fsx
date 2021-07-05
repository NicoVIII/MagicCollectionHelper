#!/bin/dotnet fsi

#r "nuget: Fake.DotNet.Cli"
#r "nuget: Fake.IO.FileSystem"

open System
open System.IO
open System.Runtime.InteropServices

open Fake.DotNet
open Fake.IO

module Config =
    let dataFolder = "data"
    let testFolder = "tests"

// kleisli composition operator for chaining
let (>=>) fun1 fun2 = fun1 >> (Result.bind fun2)

module Result =
    /// Collapses two of this simple results into one
    let collapse res1 res2 =
        match res1, res2 with
        | Ok (), Ok () -> Ok()
        | Error l1, Error l2 -> List.append l1 l2 |> Error
        | Error l, Ok ()
        | Ok (), Error l -> Error l

module Directories =
    // I need all files recursively
    // From: http://www.fssnip.net/pi/title/Recursively-find-all-files-from-a-sequence-of-directories
    let rec enumerateAllFiles dirs =
        if Seq.isEmpty dirs then
            Seq.empty
        else
            seq {
                yield! dirs |> Seq.collect Directory.EnumerateFiles

                yield!
                    dirs
                    |> Seq.collect Directory.EnumerateDirectories
                    |> enumerateAllFiles
            }

// I want to use the Result type for results of exec
let dotnet command args () =
    printfn $"> dotnet {command} {args}"

    DotNet.exec id command args
    |> (fun result -> result.OK, result)
    |> function
        | true, _ -> Ok()
        | false, result -> result.Errors |> Error

let isFsproj (file: string) = file.EndsWith ".fsproj"

module Commands =
    let restore =
        dotnet "tool" "restore" >=> dotnet "restore" ""

    let build = dotnet "build" ""

    let test () =
        Directory.EnumerateDirectories Config.testFolder
        |> Directories.enumerateAllFiles
        |> Seq.filter isFsproj
        |> Seq.map
            (fun project ->
                printfn "\nRun tests in %s:" project

                dotnet "run" $"--project \"{project}\"" ())
        |> Seq.reduce Result.collapse

    let setupTestdata () =
        if not (RuntimeInformation.IsOSPlatform OSPlatform.Linux) then
            [ "setup-testdata is only supported on linux" ]
            |> Error
        else
            // Move workspace files
            Path.Combine(Config.dataFolder, "workspace")
            |> Directory.EnumerateFiles
            |> Shell.copy "."

            // Move share files
            Path.Combine(Config.dataFolder, "share")
            |> Directory.EnumerateFiles
            |> Shell.copy (Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".local", "share", "magic-collection-helper"))

            Ok()

// We determine, what we want to execute
let execute args =
    // We read the command from arguments
    let command =
        Array.tryItem 1 args |> Option.defaultValue ""

    // Read arguments for commands
    let commandArgs =
        if Array.length args > 2 then
            Array.skip 2 args |> Array.toList
        else
            []

    (command, commandArgs)
    // We determine which commands to run
    |> function
        | "restore", [] -> Commands.restore
        | "build", [] -> Commands.build
        | "test", [] -> Commands.build >=> Commands.test
        | "setup-testdata", [] -> Commands.setupTestdata
        | _ ->
            (fun _ ->
                [ "Usage: (./run.fsx | dotnet fsi run.fsx) <command>"
                  "Look up available commands in run.fsx" ]
                |> Error)

// We execute it and map the result to an exit code
execute fsi.CommandLineArgs ()
|> function
    | Ok () -> 0
    | Error errorList ->
        errorList |> List.iter (eprintfn "%s")
        1
|> exit
