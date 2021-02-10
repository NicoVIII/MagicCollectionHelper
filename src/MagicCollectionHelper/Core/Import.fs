namespace MagicCollectionHelper.Core

[<RequireQualifiedAccess>]
module Import =
    open FSharp.Data
    open System.IO

    open MagicCollectionHelper.Core.TryParser
    open MagicCollectionHelper.Core.Types

    type Collection = CsvProvider<"./example.csv">

    // New abbreviatens taken from https://mtg.gamepedia.com/Set#List_of_Magic_expansions_and_sets
    let private convertSetAbbrev set =
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

    let private rowToEntry (row: Collection.Row): CardEntry =
        { amount = row.Amount |> uint
          name = row.Card_name
          number =
              row.Collector_number
              |> parseUint
              |> Option.map
                  (function
                  | number when
                      row.Set_code.Length = 4
                      && row.Set_code.StartsWith "T" -> number |> TokenNumber |> SetTokenNumber
                  | number -> number |> CardNumber |> SetCardNumber)
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
              | set ->
                  set
                  |> function
                  | set when set.Length = 4 && set.StartsWith "T" -> set.Substring 1
                  | set -> set
                  |> convertSetAbbrev
                  |> MagicSet
                  |> Some }

    let private parseCsv (filePath: string) =
        Collection.Load filePath
        |> (fun x -> x.Rows)
        |> Seq.map rowToEntry

    let private searchImportFile () =
        Directory.EnumerateFiles "."
        |> Seq.sortDescending
        |> Seq.tryFind (
            Path.GetFileName
            >> (fun (s: string) -> s.StartsWith "collection")
        )
        // Convert to absolute path
        |> Option.map (Path.GetFullPath)

    let perform =
        searchImportFile >> Option.map (parseCsv)
