namespace MagicCollectionHelper.Core

[<RequireQualifiedAccess>]
module CollectionImport =
    open FSharp.Data
    open System.IO

    open MagicCollectionHelper.Core.TryParser
    open MagicCollectionHelper.Core.Types

    type Collection = CsvProvider<"./example.csv">

    let private rowToEntry (row: Collection.Row): DeckStatsCardEntry =
        { amount = row.Amount |> uint
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
                  |> MagicSet.create
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

    // Because import could become an expensive task, we provide an async version
    let performAsync () =
        async {
            return perform ()
        }
