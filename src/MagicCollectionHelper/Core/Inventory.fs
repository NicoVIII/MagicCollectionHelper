namespace MagicCollectionHelper.Core

open MagicCollectionHelper.Core.Types

[<RequireQualifiedAccess>]
module Inventory =
    let fitsRule option rule =
        match option with
        | Some value -> rule value
        | None -> true

    let fitsInSetRule (card: Card) rules =
        fitsRule rules.inSet (Set.contains card.set)

    let fitsInLanguageRule (card: Card) rules =
        fitsRule rules.inLanguage (fun language -> language = card.language)

    let fitsIsFoil (card: Card) rules =
        fitsRule rules.isFoil (fun shouldBeFoil -> shouldBeFoil = card.foil)

    let fitsRarity (info: CardInfo option) rules =
        let rule rarity =
            match info with
            | None -> false
            | Some info -> Set.contains info.rarity rarity

        fitsRule rules.rarity rule

    let fitsColorIdentity (info: CardInfo option) rules =
        let rule colorIdentities =
            match info with
            | None -> false
            | Some info -> Set.contains info.colorIdentity colorIdentities

        fitsRule rules.colorIdentity rule

    let fitsLimit infoMap cardsInLoc (card: Card) rules =
        let rule limit =
            let sum =
                List.sumBy
                    (fun c ->
                        if Card.isSame infoMap c card then
                            1
                        else
                            0)
                    cardsInLoc

            (uint) sum < limit

        fitsRule rules.limit rule

    let fitsLimitExact cardsInLoc (card: Card) rules =
        let rule limitExact =
            let sum =
                List.sumBy
                    (fun (c: Card) ->
                        if c.set = card.set && c.number = card.number then
                            1
                        else
                            0)
                    cardsInLoc

            (uint) sum < limitExact

        fitsRule rules.limitExact rule

    let fitsRules (infoMap: CardInfoMap) cardsInLoc (card: Card) rules =
        let info = infoMap.TryFind(card.set, card.number)

        [ fitsInSetRule card
          fitsInLanguageRule card
          fitsIsFoil card
          fitsColorIdentity info
          fitsRarity info
          fitsLimit infoMap cardsInLoc card
          fitsLimitExact cardsInLoc card ]
        // Evaluate functions
        |> List.map (fun fnc -> fnc rules)
        |> List.forall id

    let fitsInLocation (infoMap: CardInfoMap) (locCardMap: Map<InventoryLocation, Card list>) card location =
        let cardList = locCardMap.Item(Custom location)

        fitsRules infoMap cardList card location.rules

    let determineLocation (infoMap: CardInfoMap) locCardMap locations card =
        locations
        |> Map.tryPick
            (fun _ location ->
                if fitsInLocation infoMap locCardMap card location then
                    Some location
                else
                    None)

    let newEntryForCard (card: Card) : CardEntry = { amount = 1u; card = card }

    let cardToEntryList (cardList: Card list) =
        let mutable entryList = []

        for card in cardList do
            let entry =
                List.tryFind (fun entry -> card = entry.card) entryList

            entryList <-
                match entry with
                | Some entry ->
                    let newEntry =
                        { entry with
                              amount = entry.amount + 1u }

                    newEntry :: List.except [ entry ] entryList
                | None -> (newEntryForCard card) :: entryList

        entryList

    let take (infoMap: CardInfoMap) locations entries =
        let mutable locCardMap =
            let mutable map = Map.empty
            map <- Map.add Fallback [] map

            for KeyValue (_, location) in locations do
                map <- Map.add (Custom location) [] map

            map

        // Foils at first
        let entries =
            List.sortBy (fun (entry: CardEntry) -> if entry.card.foil then 0 else 1) entries

        for entry in entries do
            for _ = 1 to (int) entry.amount do
                let location =
                    determineLocation infoMap locCardMap locations entry.card

                match location with
                | Some location ->
                    locCardMap <- Map.change (Custom location) (fun (Some l) -> entry.card :: l |> Some) locCardMap
                | None -> locCardMap <- Map.change Fallback (fun (Some l) -> entry.card :: l |> Some) locCardMap

        // Collapse into entries again
        Map.map (fun _ cardList -> cardToEntryList cardList) locCardMap

    // Because this process can take some time, we provide an async version
    let takeAsync infoMap locations entries =
        async { return take infoMap locations entries }
