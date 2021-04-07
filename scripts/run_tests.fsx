#!/bin/dotnet fsi

#r "nuget: Fake.DotNet.Cli"

open Fake.DotNet
open System.IO

#load "ressources/config.fsx"
open Config

// We run all tests we have
for testDir in Directory.EnumerateDirectories Config.testsBasePath do
    DotNet.exec id "run" $"--project {testDir}"
    |> ignore
