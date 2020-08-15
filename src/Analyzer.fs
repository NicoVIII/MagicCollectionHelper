namespace MagicCollectionHelper

open FSharp.Data
open System

open MagicCollectionHelper.TryParser
open MagicCollectionHelper.Types

module Analyzer =
    type Collection = CsvProvider<"./example.csv">

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
              | set -> MagicSet set |> Some }

    let parseCsv (filePath: string) =
        Collection.Load filePath
        |> (fun x -> x.Rows)
        |> Seq.map rowToEntry

    let combine analyzer1 analyzer2 =
        let createEmpty () =
            (analyzer2.emptyData (), analyzer1.emptyData ())

        let collect data entry =
            let result1 = analyzer1.collect (data |> snd) entry
            let result2 = analyzer2.collect (data |> fst) entry
            (result2, result1)

        let postprocess data =
            let result1 = analyzer1.postprocess (data |> snd)
            let result2 = analyzer2.postprocess (data |> fst)
            (result2, result1)

        Analyzer.create createEmpty collect postprocess

    let analyzeWith analyzer data =
        (analyzer.emptyData (), data)
        ||> Seq.fold analyzer.collect
        |> analyzer.postprocess

    let analyze filepath =
        let analyzer =
            BasicAnalyzer.get
            |> combine SetAnalyzer.get
            |> combine LanguageAnalyzer.get

        parseCsv filepath |> analyzeWith analyzer
