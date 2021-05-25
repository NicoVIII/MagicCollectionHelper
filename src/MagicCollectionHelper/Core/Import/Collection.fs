namespace MagicCollectionHelper.Core.Import

[<RequireQualifiedAccess>]
module Collection =
    open FSharp.Data
    open System.IO

    open MagicCollectionHelper.Core.Types

    type Collection = CsvProvider<"./example.csv">

    let private rowToEntry (row: Collection.Row) : DeckStatsCardEntry =
        { amount = row.Amount |> uint
          name = row.Card_name
          number =
              row.Collector_number
              |> function
              | "" -> None
              | nr -> nr |> CollectorNumber |> Some
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
              | set -> MagicSet.create set |> Some }

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

    let import =
        searchImportFile >> Option.map (parseCsv)

    // Because import could become an expensive task, we provide an async version
    let importAsync () = async { return import () }
