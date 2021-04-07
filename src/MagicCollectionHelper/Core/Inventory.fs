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

    let fitsTypeContains (info: CardInfo) rules =
        fitsRule rules.typeContains (Set.forall info.typeLine.Contains)

    let fitsTypeNotContains (info: CardInfo) rules =
        fitsRule rules.typeNotContains (Set.forall (info.typeLine.Contains >> not))

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
          fitsTypeContains cardWithInfo.info
          fitsTypeNotContains cardWithInfo.info
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
        |> List.tryFind (fitsInLocation locCardMap card)

    let newEntryForCard (card: Card) : CardEntry = { amount = 1u; card = card }

    let take (infoMap: CardInfoMap) locations entries =
        // We have to sort locations first
        let locations = CustomLocation.mapToSortedList locations

        let mutable locCardMap =
            let mutable map = Map.empty
            map <- Map.add Fallback [] map

            for location in locations do
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
                |> CardEntry.collapseCardList)
            locCardMap

    // Because this process can take some time, we provide an async version
    let takeAsync infoMap locations entries =
        async { return take infoMap locations entries }
