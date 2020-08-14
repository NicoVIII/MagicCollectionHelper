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
          language = English // TODO:
          set =
              row.Set_code
              |> function
              | "" -> None
              | set -> MagicSet set |> Some }

    let parseCsv (filePath: string) =
        Collection.Load filePath
        |> (fun x -> x.Rows)
        |> Seq.map rowToEntry

    // Basic analysis
    type BasicAnalysis =
        { amount: uint
          unique: Set<MagicSet * uint>
          foils: uint }

    module BasicAnalysis =
        let private createEmpty () =
            { amount = 0u
              unique = Set.empty
              foils = 0u }

        let private collect data (entry: CardEntry) =
            let addFoils =
                match entry.foil with
                | true -> entry.amount
                | false -> 0u

            { data with
                  amount = data.amount + entry.amount
                  foils = data.foils + addFoils }

        let analyzer = Analyzer.createBasic createEmpty collect

    // Set analysis
    type SetAnalysis = Map<MagicSet, (Set<uint> * int)>

    module SetAnalysis =
        let private createEmpty () = Map.empty

        let private collect data (entry: CardEntry) =
            // We skip cards without set
            match entry.set, entry.number with
            | Some mtgSet, Some number ->
                let set =
                    data
                    |> Map.tryFind mtgSet
                    |> Option.defaultValue Set.empty
                    |> Set.add number

                data |> Map.add mtgSet set
            | _ -> data

        let private postprocess data: SetAnalysis =
            data
            |> Map.map (fun _ set -> (set, Set.count set))

        let analyzer =
            Analyzer.create createEmpty collect postprocess

    let combine analyzer1 analyzer2 =
        let createEmpty () =
            (analyzer1.emptyData (), analyzer2.emptyData ())

        let collect data entry =
            let result1 = analyzer1.collect (data |> fst) entry
            let result2 = analyzer2.collect (data |> snd) entry
            (result1, result2)

        let postprocess data =
            let result1 = analyzer1.postprocess (data |> fst)
            let result2 = analyzer2.postprocess (data |> snd)
            (result1, result2)

        Analyzer.create createEmpty collect postprocess

    let analyzeWith analyzer data =
        (analyzer.emptyData (), data)
        ||> Seq.fold analyzer.collect
        |> analyzer.postprocess

    let analyze filepath =
        let analyzer =
            combine BasicAnalysis.analyzer SetAnalysis.analyzer

        parseCsv filepath |> analyzeWith analyzer
