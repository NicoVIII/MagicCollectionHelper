namespace MagicCollectionHelper.Core

open System.Text

open MagicCollectionHelper.Core.Types

module SetAnalyser =
    type CollectionData<'a> =
        { collected: uint32
          max: 'a
          percent: float }

    type CardSetData =
        { cards: CollectionData<CardNumber>
          missing: Set<SetNumber>
          name: string
          token: CollectionData<TokenNumber> option }

    type ResultValue =
        { cards: Set<SetNumber>
          setData: CardSetData option }

    type Result = Map<MagicSet, ResultValue>

    type CollectType = Map<MagicSet, Set<SetNumber>>

    type Preferences =
        { missingPercent: float
          dozenalize: bool
          withFoils: bool }

    let private createEmpty (): CollectType = Map.empty

    let private collect settings (data: CollectType) (entry: DeckStatsCardEntry): CollectType =
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

        let transformSet (setData: SetDataMap) set numberSet =
            let setData =
                setData.TryFind set
                |> Option.map
                    (fun { name = name
                           maxCard = max
                           maxToken = maxToken } ->
                        let cardData =
                            let collected =
                                numberSet
                                // We have to remove cards outside of the normal number range from the collected number
                                |> Set.filter
                                    (fun number ->
                                        match number with
                                        | SetCardNumber (CardNumber number) -> number > 0u && number <= max.Value
                                        | _ -> false)
                                |> Set.count
                                |> uint32

                            { collected = collected
                              max = max
                              percent = calcPercent collected max.Value }

                        // If we have the Token max for this set, we generate token data
                        let tokenData =
                            match maxToken with
                            | Some maxToken ->
                                let collected =
                                    numberSet
                                    // We have to remove cards outside of the token number range from the collected number
                                    |> Set.filter
                                        (fun number ->
                                            match number with
                                            | SetTokenNumber (TokenNumber number) ->
                                                number > 0u && number <= maxToken.Value
                                            | _ -> false)
                                    |> Set.count
                                    |> uint32

                                { collected = collected
                                  max = maxToken
                                  percent = calcPercent collected maxToken.Value }
                                |> Some
                            | None -> None

                        // Missing Cards for complete collection
                        let missingCards =
                            seq { 1u .. max.Value }
                            |> Set.ofSeq
                            |> Set.map SetNumber.Card
                            |> Set.filter (fun x -> numberSet |> (Set.contains x >> not))

                        let missingToken =
                            match maxToken with
                            | Some maxToken ->
                                seq { 1u .. maxToken.Value }
                                |> Set.ofSeq
                                |> Set.map SetNumber.Token
                                |> Set.filter (fun x -> numberSet |> (Set.contains x >> not))
                            | None -> Set.empty

                        let missing = missingCards |> Set.union missingToken

                        { cards = cardData
                          missing = missing
                          name = name
                          token = tokenData })

            { cards = numberSet; setData = setData }

    let private postprocess (cardData: SetDataMap) (data: CollectType): Result =
        data
        |> Map.map (Postprocess.transformSet cardData)

    module private Print =
        let inline private setLine dozenalize unwrap (set: MagicSet) setName collectionData =
            let percent =
                if dozenalize then
                    collectionData.percent * 144.
                else
                    collectionData.percent * 100.

            [ sprintf
                "%5s - %3s/%3s (%s%s)"
                set.Value
                (Numbers.print dozenalize 0 (int collectionData.collected))
                (Numbers.print dozenalize 0 (collectionData.max |> unwrap |> int))
                (Numbers.print dozenalize 1 percent)
                (if dozenalize then "pg" else "%")
              match setName with
              | Some setName -> $" - %s{setName}"
              | None -> () ]
            |> List.reduce (+)
            |> Seq.singleton

        let cardSetLine dozenalize set setName =
            setLine dozenalize CardNumber.unwrap set (Some setName)

        let tokenSetLine dozenalize (set: MagicSet) =
            setLine dozenalize TokenNumber.unwrap ("T" + set.Value |> MagicSet.create) None

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
                    |> Option.map (fun setData -> setData.cards.percent * -100.0) // Use negative numbers
                    |> Option.defaultValue (
                        let cards, token = value.cards |> SetNumber.splitSet
                        let cardsValue = cards.Count |> (-) 1000 |> double // Stay in positive numbers

                        let tokenValue = token.Count |> double |> (*) -0.001

                        cardsValue + tokenValue
                    ))

        let titleLine = "Set Analysis" |> Seq.singleton

        let setLines =
            setsSorted
            |> Seq.map
                (fun (set, value) ->
                    match value.setData with
                    | Some setData ->
                        // Print set line and maybe token set line
                        [ Print.cardSetLine dozenalize set setData.name setData.cards
                          match setData.token with
                          | Some ({ collected = collected } as tokenData) when collected > 0u ->
                              Print.tokenSetLine dozenalize set tokenData
                          | _ -> () ]
                        |> Seq.concat
                    | None ->
                        let cards, token = value.cards |> SetNumber.splitSet

                        $"%5s{set.Value} - No set data found (%2i{cards.Count} cards/%2i{token.Count} token)"
                        |> Seq.singleton)
            |> Seq.concat

        let missingLines =
            setsSorted
            |> Seq.choose
                (fun (set, value) ->
                    let setData = value.setData

                    match setData with
                    | Some ({ missing = missing
                              cards = { percent = percent } } as setData) when
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

                    let cardIds, tokenIds = setData.missing |> SetNumber.splitSeq

                    let mutable missingLines = Seq.empty

                    if Seq.length cardIds > 0 then
                        missingLines <-
                            cardIds
                            |> Seq.map (fun (CardNumber number) -> number |> Numbers.print dozenalize 0)
                            |> Seq.reduce (fun x y -> x + "," + y)
                            |> Seq.singleton
                            |> Seq.append missingLines

                    if Seq.length tokenIds > 0 then
                        missingLines <-
                            tokenIds
                            |> Seq.map (fun (TokenNumber number) -> number |> Numbers.print dozenalize 0)
                            |> Seq.reduce (fun x y -> x + "," + y)
                            |> (+) "Token: "
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
