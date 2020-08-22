namespace MagicCollectionHelper

open FSharp.Data
open System

open MagicCollectionHelper.TryParser
open MagicCollectionHelper.Types

module Analyser =
    type Collection = CsvProvider<"./example.csv">

    // New abbreviatens taken from https://mtg.gamepedia.com/Set#List_of_Magic_expansions_and_sets
    let convertSetAbbrev set =
        match set with
        | "1E" -> "LEA" // Alpha (Limited Edition)
        | "2E" -> "LEB" // Beta (Limited Edition)
        | "2U" -> "2ED" // Unlimited Edition
        | "AN" -> "ARN" // Arabian Nights
        | "AQ" -> "ATQ" // Antiquities
        | "3E" -> "3ED" // Revised Edition
        | "LE" -> "LEG" // Legends
        | "DK" -> "DRK" // The Dark
        | "FE" -> "FEM" // Fallen Empires
        | "4E" -> "4ED" // Fourth Edition
        | "IA" -> "ICE" // Ice Age
        | "CH" -> "CHR" // Chronicles
        | "HM" -> "HML" // Homelands
        | "AL" -> "ALL" // Alliances
        | "MI" -> "MIR" // Mirage
        | "VI" -> "VIS" // Visions
        | "5E" -> "5ED" // Fifth Edition
        | "PO" -> "POR" // Portal
        | "WL" -> "WTH" // Weatherlight
        | "TE" -> "TMP" // Tempest
        | "ST" -> "STH" // Stronghold
        | "EX" -> "EXO" // Exodus
        | "P2" -> "P02" // Portal Second Age
        | "UG" -> "UGL" // Unglued
        | "UZ" -> "USG" // Urza's Saga
        | "UL" -> "ULG" // Urza's Legacy
        | "6E" -> "6ED" // Sixth Edition
        | "PK" -> "PTK" // Portal Three Kingdoms
        | "UD" -> "UDS" // Urza's Destiny
        | "P3" -> "S99" // Starter 1999
        | "MM" -> "MMQ" // Mercadian Masques
        | "NE" -> "NEM" // Nemesis
        | "PR" -> "PCY" // Prophecy
        | "IN" -> "INV" // Invasion
        | "PS" -> "PLS" // Planeshift
        | "7E" -> "7ED" // Seventh Edition
        | "AP" -> "APC" // Apocalypse
        | "OD" -> "ODY" // Odyssey
        | _ -> set

    let rowToEntry (row: Collection.Row) =
        { amount = row.Amount |> uint
          name = row.Card_name
          number = row.Collector_number |> parseUint
          foil = row.Is_foil.GetValueOrDefault() = 1
          language =
              row.Language
              |> function
              | "" -> None
              | set -> Language set |> Some
          set =
              row.Set_code
              |> function
              | "" -> None
              | set -> set |> convertSetAbbrev |> MagicSet |> Some }

    let parseCsv (filePath: string) =
        Collection.Load filePath
        |> (fun x -> x.Rows)
        |> Seq.map rowToEntry

    let combine analyser1 analyser2 =
        let createEmpty () =
            (analyser2.emptyData (), analyser1.emptyData ())

        let collect data entry =
            let result1 = analyser1.collect (data |> snd) entry
            let result2 = analyser2.collect (data |> fst) entry
            (result2, result1)

        let postprocess data =
            let result1 = analyser1.postprocess (data |> snd)
            let result2 = analyser2.postprocess (data |> fst)
            (result2, result1)

        Analyser.create createEmpty collect postprocess

    let analyseWith analyser data =
        (analyser.emptyData (), data)
        ||> Seq.fold analyser.collect
        |> analyser.postprocess

    let analyse filepath =
        let analyser =
            BasicAnalyser.get
            |> combine SetAnalyser.get
            |> combine LanguageAnalyser.get

        parseCsv filepath |> analyseWith analyser
