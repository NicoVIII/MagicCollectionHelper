namespace MagicCollectionHelper

open MagicCollectionHelper.ErrorHandling
open MagicCollectionHelper.Types

open System.IO

/// This module handles the command line call and prepares the arguments for
/// the analyzer
module Program =
    /// Prepares command line arguments
    let handleArguments argv =
        // Atm we just accept a filepath for the csv file
        match argv with
        | [| filePath |] ->
            let filePath = Path.GetFullPath filePath

            if File.Exists filePath then Ok { filePath = filePath } else Error NonExistingFile
        | _ -> Error InvalidArguments

    [<EntryPoint>]
    let main argv =
        // Check, if arguments are valid
        match handleArguments argv with
        | Ok arguments ->
            let (basicResult, setResult), languageResult = Analyser.analyse arguments.filePath

            // Print Setresults
            printfn "Set Analysis"
            setResult
            |> Map.map (fun (CardSet key) value ->
                // TODO: move this step to the postprocess of the analyser
                let setDataExt =
                    CardData.setData.TryFind key
                    |> Option.map (fun (name, max, _) ->
                        // We have to remove cards outside of the normal number range from the collected number
                        let collected =
                            value
                            |> Set.filter (fun el -> el > 0u && el <= max)
                            |> Set.count

                        let percent =
                            (collected, max)
                            ||> (fun x y -> x |> double, y |> double)
                            ||> (/)
                            |> (*) 100.0

                        // Missing Cards only for nearly complete sets
                        let missing =
                            match percent with
                            | percent when percent > 75.0 ->
                                seq { 1u .. max }
                                |> Seq.filter (fun x -> value |> Set.contains x |> not)
                                |> Seq.toList
                            | _ -> []

                        {| max = max
                           name = name
                           percent = percent |},
                        collected,
                        missing)

                let setData, collected, missing =
                    setDataExt
                    |> function
                    | Some (setData, collected, missing) -> Some setData, collected, missing
                    | None -> None, value |> Set.count, []

                {| cards = value
                   collected = collected
                   missing = missing
                   setData = setData |})
            |> Map.toList
            |> List.sortBy (fun (_, value) ->
                value.setData
                |> Option.map (fun setData -> setData.percent * -1.0)
                |> Option.defaultValue 0.0)
            |> List.iter (fun ((CardSet key), value) ->
                let setMax, percent, setName =
                    value.setData
                    |> function
                    | Some setData ->
                        let max = setData.max |> string

                        let percent = setData.percent |> sprintf "%.1f"

                        max, percent, setData.name
                    | _ -> "? ", "? ", ""

                printfn "%-3s - %3i/%3s (%5s%%) - %s" key value.collected setMax percent setName
                match value.missing with
                | [] -> ()
                | missing ->
                    printfn "Missing (%2i):" missing.Length
                    missing
                    |> List.map string
                    |> List.reduce (fun x y -> x + "," + y)
                    |> printfn "%s"
                    printfn "")
            printfn ""

            // Print Languageresults
            printfn "Language Analysis"
            languageResult
            |> Map.iter (fun (Language key) value -> printfn "%s: %i" key value)
            printfn ""

            // Print Basicresults
            printfn "Basic Analysis"
            printfn "%5i - Amount" basicResult.amount
            printfn "%5i - With Language" basicResult.withLanguage
            printfn "%5i - With Set" basicResult.withSet
            printfn "%5i - With Set (unique)" basicResult.uniqueWithSet
            printfn "%5i - Foils" basicResult.foils
            0
        | Error error -> handleError error
