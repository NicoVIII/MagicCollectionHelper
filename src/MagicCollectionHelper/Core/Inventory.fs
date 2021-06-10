namespace MagicCollectionHelper.Core

open System

open MagicCollectionHelper.Core.Types
open MagicCollectionHelper.Core.Types.Generated

[<RequireQualifiedAccess>]
module Inventory =
    module Rules =
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

        let fitsAll cardsInLoc (cardWithInfo: CardWithInfo) rules =
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

    let fitsInLocation (locCardMap: Map<InventoryLocation, Oldable<CardWithInfo> list>) card location =
        let cardList =
            locCardMap.Item(Custom location)
            |> List.map Oldable.data

        Rules.fitsAll cardList card location.rules

    let determineLocation locCardMap locations card =
        locations
        |> List.tryFind (fitsInLocation locCardMap card)

    let getSortByValue setData (entryWithInfo: CardEntryWithInfo) sortBy =
        let entry = entryWithInfo.entry
        let info = entryWithInfo.info

        match sortBy with
        | ByColorIdentity ->
            let pos =
                ColorIdentity.getPosition info.colorIdentity

            sprintf "%02i" pos
        | ByName -> info.name
        | BySet ->
            let date =
                Map.tryFind entry.card.set setData
                |> function
                | Some setData -> setData.date
                | None -> "0000-00-00"

            let extension =
                match entry.card.set.Value with
                | set when set.StartsWith "T" -> set.Substring 1 + "Z"
                | set -> set + "A"

            $"{date}{extension}"
        | ByCollectorNumber -> sprintf "%s" (entry.card.number.Value.PadLeft(3, '0'))
        | ByCmc -> sprintf "%02i" info.cmc
        | ByTypeContains typeContains ->
            typeContains
            |> List.fold
                (fun (found, strng) typeContains ->
                    if found then
                        (true, strng + "9")
                    else if info.typeLine.Contains typeContains then
                        (true, strng + "1")
                    else
                        (false, strng + "9"))
                (false, "")
            |> snd
        | ByRarity rarities ->
            rarities
            |> List.indexed
            |> List.tryPick
                (fun (index, raritySet) ->
                    if Set.contains info.rarity raritySet then
                        Some index
                    else
                        None)
            |> Option.defaultValue (List.length rarities)
            |> string
        | ByLanguage language ->
            language
            |> List.indexed
            |> List.tryPick
                (fun (index, language) ->
                    if language = entry.card.language then
                        Some index
                    else
                        None)
            |> Option.defaultValue (List.length language)
            |> string

    let sortEntries setData location entries =
        let random = Random()

        let sortRules =
            match location with
            | Custom location -> location.sortBy
            | Fallback -> [ ByName ]

        let sortBy (e: OldAmountable<CardEntryWithInfo>) =
            sortRules
            |> List.map (getSortByValue setData e.data)
            // We add a random factor at the end
            |> (fun lst -> List.append lst [ random.Next(0, 10000) |> sprintf "%04i" ])

        List.sortBy sortBy entries

    let take (setData: SetDataMap) (infoMap: CardInfoMap) locations (entries: OldAmountable<CardEntry> list) =
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
                (fun (oldable: OldAmountable<CardEntry>) ->
                    let entry = oldable.data
                    let amountOld = oldable.amountOld

                    Map.tryFind (entry.card.set, entry.card.number) infoMap
                    |> Option.map (fun info -> OldAmountable.create amountOld { entry = entry; info = info }))
            // TODO: generalize and make configurable
            |> List.sortBy
                (fun oldAmountable ->
                    let entryWithInfo = oldAmountable.data

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

        for oldAmountable in entriesWithInfo do
            let entryWithInfo = oldAmountable.data

            let cardWithInfo =
                CardWithInfo.create entryWithInfo.entry.card entryWithInfo.info

            let old = Oldable.create true cardWithInfo

            let notOld = Oldable.create false cardWithInfo

            // for .. to does not work for uint
            // BEWARE: Very unfunctional code ahead!
            let mutable i = 0u

            while i < entryWithInfo.entry.amount do
                let oldable =
                    if i < oldAmountable.amountOld then
                        old
                    else
                        notOld

                let location =
                    determineLocation locCardMap locations cardWithInfo

                match location with
                | Some location ->
                    locCardMap <- Map.change (Custom location) (Option.map (fun l -> oldable :: l)) locCardMap
                | None -> locCardMap <- Map.change Fallback (Option.map (fun l -> oldable :: l)) locCardMap

                i <- i + 1u

        // Collapse into entries again and sort by location rules
        locCardMap
        |> Map.map
            (fun location cardList ->
                cardList
                |> CardEntryWithInfo.collapseCardList
                |> sortEntries setData location
                |> List.map (OldAmountable.map CardEntryWithInfo.entry))

        // We now convert the map back into a list (order matters!) and sort it
        |> Map.toList
        |> List.sortBy
            (fun (location, _) ->
                match location with
                | Fallback -> 9999
                | Custom location -> List.findIndex (fun l -> location = l) locations)

    // Because this process can take some time, we provide an async version
    let takeAsync setData infoMap locations entries =
        async { return take setData infoMap locations entries }
