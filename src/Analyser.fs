namespace MagicCollectionHelper

open FSharp.Data
open System

open MagicCollectionHelper.TryParser
open MagicCollectionHelper.Types

module Analyser =
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
