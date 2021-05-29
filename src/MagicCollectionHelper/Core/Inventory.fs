namespace MagicCollectionHelper.Core

open System

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

    let fitsIsToken (card: Card) rules =
        fitsRule rules.isToken (fun shouldBeToken -> shouldBeToken = Card.isToken card)

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
          fitsIsToken cardWithInfo.card
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

    let take (setData: SetDataMap) (infoMap: CardInfoMap) locations (entries: CardEntry list) =
        // We have to sort locations first
        let locations = CustomLocation.mapToSortedList locations

        let mutable locCardMap =
            let mutable map = Map.empty
            map <- Map.add Fallback [] map

            for location in locations do
                map <- Map.add (Custom location) [] map

            map

        // We sort the cards before trying to put them into locations
        let random = Random()

        let entriesWithInfo =
            entries
            // We can consider a card only for inventory, if we have the info
            |> List.choose
                (fun (entry: CardEntry) ->
                    Map.tryFind (entry.card.set, entry.card.number) infoMap
                    |> Option.map (fun info -> { entry = entry; info = info }))
            // TODO: generalize and make configurable
            |> List.sortBy
                (fun entryWithInfo ->
                    [
                      // Language
                      match entryWithInfo.entry.card.language.Value with
                      | "en" -> "0"
                      | "de" -> "1"
                      | _ -> "2"
                      // Foil
                      if entryWithInfo.entry.card.foil then
                          "0"
                      else
                          "1"
                      // Set
                      (Map.find entryWithInfo.entry.card.set setData)
                          .date
                      entryWithInfo.entry.card.number.Value
                      // Random
                      random.Next(0, 10000) |> string ])

        for entryWithInfo in entriesWithInfo do
            let cardWithInfo =
                CardWithInfo.create entryWithInfo.entry.card entryWithInfo.info

            for _ = 1 to (int) entryWithInfo.entry.amount do
                let location =
                    determineLocation locCardMap locations cardWithInfo

                match location with
                | Some location ->
                    locCardMap <- Map.change (Custom location) (Option.map (fun l -> cardWithInfo :: l)) locCardMap
                | None -> locCardMap <- Map.change Fallback (Option.map (fun l -> cardWithInfo :: l)) locCardMap

        // Collapse into entries again
        Map.map
            (fun _ cardList ->
                cardList
                |> List.map CardWithInfo.card
                |> CardEntry.collapseCardList)
            locCardMap

    // Because this process can take some time, we provide an async version
    let takeAsync setData infoMap locations entries =
        async { return take setData infoMap locations entries }
