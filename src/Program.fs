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
            setResult
            |> Map.map (fun (MagicSet key) value ->
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

                        {| max = max
                           name = name
                           percent = percent |},
                        collected)

                let setData, collected =
                    setDataExt
                    |> function
                    | Some (setData, collected) -> Some setData, collected
                    | None -> None, value |> Set.count

                {| cards = value
                   collected = collected
                   setData = setData |})
            |> Map.toList
            |> List.sortBy (fun (_, value) ->
                value.setData
                |> Option.map (fun setData -> setData.percent * -1.0)
                |> Option.defaultValue 0.0)
            |> List.iter (fun ((MagicSet key), value) ->
                let setMax, percent, setName =
                    value.setData
                    |> function
                    | Some setData ->
                        let max = setData.max |> string

                        let percent = setData.percent |> sprintf "%.1f"

                        max, percent, setData.name
                    | _ -> "? ", "? ", ""

                printfn "%-3s - %3i/%3s (%5s%%) - %s" key value.collected setMax percent setName)

            // Print Languageresults
            languageResult
            |> Map.iter (fun (Language key) value -> printfn "%s: %i" key value)

            // Print Basicresults
            printfn "%A" basicResult
            0
        | Error error -> handleError error
