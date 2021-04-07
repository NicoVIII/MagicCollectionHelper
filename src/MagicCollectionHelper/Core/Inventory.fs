namespace MagicCollectionHelper.Core

open MagicCollectionHelper.Core.Types
open MagicCollectionHelper.Core.Types.Generated

[<RequireQualifiedAccess>]
module Inventory =
    let fitsRule option rule =
        match option with
        | None -> true
        | Some value -> rule value

    let fitsInSetRule (card: Card) rules =
        fitsRule rules.inSet (Set.contains card.set)

    let fitsInLanguageRule (card: Card) rules =
        fitsRule rules.inLanguage (fun language -> language = card.language)

    let fitsIsFoil (card: Card) rules =
        fitsRule rules.isFoil (fun shouldBeFoil -> shouldBeFoil = card.foil)

    let fitsRarity (info: CardInfo) rules =
        fitsRule rules.rarity (fun rarity -> Set.contains info.rarity rarity)

    let fitsColorIdentity (info: CardInfo) rules =
        fitsRule rules.colorIdentity (fun colorIdentities -> Set.contains info.colorIdentity colorIdentities)

    let fitsLimit cardsInLoc (card: CardWithInfo) rules =
        let rule limit =
            let sum =
                List.sumBy
                    (fun c ->
                        if CardWithInfo.isSame c card then
                            1
                        else
                            0)
                    cardsInLoc

            (uint) sum < limit

        fitsRule rules.limit rule

    let fitsLimitExact cardsInLoc (card: CardWithInfo) rules =
        let rule limitExact =
            let sum =
                List.sumBy
                    (fun c ->
                        if CardWithInfo.isExactSame c card then
                            1
                        else
                            0)
                    cardsInLoc

            (uint) sum < limitExact

        fitsRule rules.limitExact rule

    let fitsRules cardsInLoc (cardWithInfo: CardWithInfo) rules =
        [ fitsInSetRule cardWithInfo.card
          fitsInLanguageRule cardWithInfo.card
          fitsIsFoil cardWithInfo.card
          fitsColorIdentity cardWithInfo.info
          fitsRarity cardWithInfo.info
          fitsLimit cardsInLoc cardWithInfo
          fitsLimitExact cardsInLoc cardWithInfo ]
        // Evaluate functions
        |> List.map (fun fnc -> fnc rules)
        |> List.forall id

    let fitsInLocation (locCardMap: Map<InventoryLocation, CardWithInfo list>) card location =
        let cardList = locCardMap.Item(Custom location)

        fitsRules cardList card location.rules

    let determineLocation locCardMap locations card =
        locations
        |> Map.tryPick
            (fun _ location ->
                if fitsInLocation locCardMap card location then
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
            let cardInfo =
                Map.tryFind (entry.card.set, entry.card.number) infoMap

            // We can consider a card only for inventory, if we have the info
            match cardInfo with
            | Some cardInfo ->
                let cardWithInfo = { card = entry.card; info = cardInfo }

                for _ = 1 to (int) entry.amount do
                    let location =
                        determineLocation locCardMap locations cardWithInfo

                    match location with
                    | Some location ->
                        locCardMap <- Map.change (Custom location) (Option.map (fun l -> cardWithInfo :: l)) locCardMap
                    | None -> locCardMap <- Map.change Fallback (Option.map (fun l -> cardWithInfo :: l)) locCardMap
            | None -> ()

        // Collapse into entries again
        Map.map
            (fun _ cardList ->
                cardList
                |> List.map CardWithInfo.card
                |> cardToEntryList)
            locCardMap

    // Because this process can take some time, we provide an async version
    let takeAsync infoMap locations entries =
        async { return take infoMap locations entries }
