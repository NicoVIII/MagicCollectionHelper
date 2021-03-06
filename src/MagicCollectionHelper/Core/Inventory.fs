namespace MagicCollectionHelper.Core

open MagicCollectionHelper.Core.Types

[<RequireQualifiedAccess>]
module Inventory =
    let fitsRule (infoMap: CardInfoMap) cardsInLoc (card: Card) rule =
        let info = infoMap.TryFind(card.set, card.number)

        match rule with
        | InSet set -> Set.contains card.set set
        | InLanguage lang -> lang = card.language
        | IsFoil foil -> card.foil = foil
        | ColorIdentity colors ->
            match info with
            | None -> false
            | Some info -> Set.contains info.colorIdentity colors
        | Limit limit ->
            let sum =
                List.sumBy
                    (fun c ->
                        if Card.isSame infoMap c card then
                            1
                        else
                            0)
                    cardsInLoc

            (uint) sum < limit

    let fitsInLocation (infoMap: CardInfoMap) (locCardMap: Map<InventoryLocation, Card list>) card location =
        let cardList = locCardMap.Item(Custom location)

        location.rules
        |> List.forall (fitsRule infoMap cardList card)

    let determineLocation (infoMap: CardInfoMap) locCardMap locations card =
        locations
        |> List.tryFind (fitsInLocation infoMap locCardMap card)

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

            for location in locations do
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
