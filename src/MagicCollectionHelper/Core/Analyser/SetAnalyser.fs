namespace MagicCollectionHelper.Core

open MagicCollectionHelper.Core.TryParser

module SetAnalyser =
    type CardSetData =
        { collected: uint32
          missing: Set<CollectorNumber>
          name: string
          max: uint
          percent: float }

    type ResultValue =
        { cards: Set<CollectorNumber>
          setData: CardSetData option }

    type Result = Map<MagicSet, ResultValue>

    type CollectType = Map<MagicSet, Set<CollectorNumber>>

    type Preferences =
        { missingPercent: float
          dozenalize: bool
          withFoils: bool }

    let private createEmpty () : CollectType = Map.empty

    let private collect settings (data: CollectType) (entry: DeckStatsCardEntry) : CollectType =
        // We skip cards without set
        match entry.set, entry.number with
        | Some mtgSet, Some number ->
            if not entry.foil || settings.withFoils then
                let set =
                    data
                    |> Map.tryFind mtgSet
                    |> Option.defaultValue Set.empty
                    |> Set.add number

                data |> Map.add mtgSet set
            else
                data
        | _ -> data

    module private Postprocess =
        let inline calcPercent collected max = (float collected) / (float max)

        let transformSet (setData: SetDataMap) set (numberSet: CollectorNumber Set) =
            let setData =
                setData.TryFind set
                |> Option.map
                    (fun { name = name; max = max } ->
                        let collected =
                            numberSet
                            // We have to remove cards outside of the normal number range from the collected number
                            |> Set.filter
                                (fun number ->
                                    match number.Value with
                                    | Uint number -> number > 0u && number <= max
                                    | _ -> false)
                            |> Set.count
                            |> uint32

                        // Missing Cards for complete collection
                        let missing =
                            seq { 1u .. max }
                            |> Seq.map (string >> CollectorNumber)
                            |> Set.ofSeq
                            |> Set.filter (fun x -> Set.contains x numberSet |> not)

                        { missing = missing
                          name = name
                          collected = collected
                          max = max
                          percent = calcPercent collected max })

            { cards = numberSet; setData = setData }

    let private postprocess (cardData: SetDataMap) (data: CollectType) : Result =
        data
        |> Map.map (Postprocess.transformSet cardData)

    module private Print =
        let setLine dozenalize (set: MagicSet) collectionData =
            let percent =
                if dozenalize then
                    collectionData.percent * 144.
                else
                    collectionData.percent * 100.

            sprintf
                "%5s - %3s/%3s (%s%s) - %s"
                set.Value
                (Numbers.print dozenalize 0 (int collectionData.collected))
                (Numbers.print dozenalize 0 (collectionData.max |> int))
                (Numbers.print dozenalize 1 percent)
                (if dozenalize then "pg" else "%")
                collectionData.name
            |> Seq.singleton

    let private print (settings: Preferences) (result: Result) =
        let dozenalize = settings.dozenalize

        // We sort sets descending by "fullness"
        // If we have no set data we sort by number of cards + tokens, then number of cards
        let setsSorted =
            result
            |> Map.toSeq
            |> Seq.sortBy
                (fun (_, value) ->
                    value.setData
                    |> Option.map (fun setData -> setData.percent * -100.0) // Use negative numbers
                    |> Option.defaultValue (
                        value.cards.Count |> (-) 1000 |> double // Stay in positive numbers
                    ))

        let titleLine = "Set Analysis" |> Seq.singleton

        let setLines =
            setsSorted
            |> Seq.map
                (fun (set, value) ->
                    match value.setData with
                    | Some setData ->
                        // Print set line and maybe token set line
                        Print.setLine dozenalize set setData
                    | None ->
                        $"%5s{set.Value} - No set data found (%2i{value.cards.Count})"
                        |> Seq.singleton)
            |> Seq.concat

        let missingLines =
            setsSorted
            |> Seq.choose
                (fun (set, value) ->
                    let setData = value.setData

                    match setData with
                    | Some ({ missing = missing; percent = percent } as setData) when
                        percent > settings.missingPercent
                        && missing.Count > 0 -> Some(set, setData)
                    | _ -> None)
            |> Seq.map
                (fun (set: MagicSet, setData) ->
                    let missing =
                        setData.missing.Count
                        |> Numbers.print dozenalize 0

                    let titleLine =
                        $"%-3s{set.Value} - %2s{missing} missing:"
                        |> Seq.singleton

                    let cardIds = setData.missing

                    let mutable missingLines = Seq.empty

                    if Seq.length cardIds > 0 then
                        missingLines <-
                            cardIds
                            |> Seq.map (fun (CollectorNumber number) -> number |> Numbers.print dozenalize 0)
                            |> Seq.reduce (fun x y -> x + "," + y)
                            |> Seq.singleton
                            |> Seq.append missingLines

                    [ titleLine; missingLines ] |> Seq.concat)
            |> Seq.reduce (fun x y -> [ x; Seq.singleton ""; y ] |> Seq.concat)

        [ titleLine
          setLines
          Seq.singleton ""
          missingLines ]
        |> Seq.concat

    let get =
        Analyser.create createEmpty collect postprocess print
