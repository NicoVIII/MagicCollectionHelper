namespace MagicCollectionHelper

open System.Text

open MagicCollectionHelper.Types

module SetAnalyser =
    type Result = Map<MagicSet, Set<uint>>

    type CollectType = Map<MagicSet, Set<uint>>

    type Settings = { missingPercent: float }

    let private createEmpty (): CollectType = Map.empty

    let private collect (data: CollectType) (entry: CardEntry): CollectType =
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

    let private postprocess (data: CollectType): Result = data

    let private print (settings: Settings) (result: Result) =
        let title = "Set Analysis" |> Seq.singleton

        // Split Map into card set and token set map
        let cardMap, tokenMap =
            result
            |> Map.fold (fun (cardMap, tokenMap) set value ->
                match set with
                | SetOfCards cardSet ->
                    let newMap = cardMap |> Map.add cardSet value
                    (newMap, tokenMap)
                | SetOfToken tokenSet ->
                    let newMap = tokenMap |> Map.add tokenSet value
                    (cardMap, newMap)) (Map.empty, Map.empty)

        let data =
            // TODO: Refactor, nobody gets what happens here!
            cardMap
            |> Map.map (fun set value ->
                let setDataExt =
                    CardData.tryFindByCardSet set
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
                            | percent when percent > settings.missingPercent ->
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
            |> Map.toSeq
            |> Seq.sortBy (fun (_, value) ->
                value.setData
                |> Option.map (fun setData -> setData.percent * -1.0)
                |> Option.defaultValue 0.0)
            |> Seq.map (fun ((CardSet key), value) ->
                let setMax, percent, setName =
                    value.setData
                    |> function
                    | Some setData ->
                        let max = setData.max |> string

                        let percent = setData.percent |> sprintf "%.1f"

                        max, percent, setData.name
                    | _ -> "? ", "? ", ""

                let mutable out =
                    sprintf "%-3s - %3i/%3s (%5s%%) - %s" key value.collected setMax percent setName
                    |> Seq.singleton

                match value.missing with
                | [] -> ()
                | missing ->
                    let title =
                        sprintf "Missing (%2i): " missing.Length
                        |> Seq.singleton

                    let ids =
                        missing
                        |> Seq.map string
                        |> Seq.reduce (fun x y -> x + "," + y)
                        |> Seq.singleton

                    out <-
                        [ out; title; ids; Seq.singleton "" ]
                        |> Seq.concat

                out)
            |> Seq.concat

        [ title; data ] |> Seq.concat

    let get =
        Analyser.create createEmpty collect postprocess print
