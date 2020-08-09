namespace MagicCollectionHelper

open FSharp.Data

open MagicCollectionHelper.Types

module Analyzer =
    type Collection = CsvProvider<"./example.csv">

    let rowToEntry (row: Collection.Row) =
        { amount = row.Amount |> uint
          name = row.Card_name
          number = row.Collector_number
          foil = row.Is_foil.GetValueOrDefault() = 1
          language = English // TODO:
          set = Core2020 } // TODO:

    let parseCsv (filePath: string) =
        Collection.Load filePath
        |> (fun x -> x.Rows)
        |> Seq.map rowToEntry

    // Basic analysis
    type BasicAnalysis =
        { amount: uint
          unique: uint
          // TODO: uniqueSet: uint
          foils: uint }

    module BasicAnalysis =
        let createEmpty () = { amount = 0u; unique = 0u; foils = 0u }

        let analyze data (entry: CardEntry) =
            let addFoils =
                match entry.foil with
                | true -> entry.amount
                | false -> 0u

            { data with
                  amount = data.amount + entry.amount
                  foils = data.foils + addFoils }

    let basicAnalyzer =
        Analyzer.create BasicAnalysis.createEmpty BasicAnalysis.analyze

    let analyzeWith analyzer =
        Seq.fold analyzer.analyze (analyzer.emptyData ())

    let analyze filepath =
        parseCsv filepath
        // We do some basic stuff for now
        |> analyzeWith basicAnalyzer
