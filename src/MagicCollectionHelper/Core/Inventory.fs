namespace MagicCollectionHelper.Core

open System

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
            [ fitsInSetRule cardWithInfo.data
              fitsInLanguageRule cardWithInfo.data
              fitsIsFoil cardWithInfo.data
              fitsIsToken cardWithInfo.data
              fitsTypeContains cardWithInfo.info
              fitsTypeNotContains cardWithInfo.info
              fitsColorIdentity cardWithInfo.info
              fitsRarity cardWithInfo.info
              fitsLimit cardsInLoc cardWithInfo
              fitsLimitExact cardsInLoc cardWithInfo ]
            // Evaluate functions
            |> List.map (fun fnc -> fnc rules)
            |> List.forall id

    let fitsInLocation (locCardMap: Map<InventoryLocation, AgedCardWithInfo list>) card location =
        let cardList =
            locCardMap.Item(Custom location)
            |> List.map (WithInfo.map Oldable.data)

        Rules.fitsAll cardList card location.rules

    let determineLocation locCardMap locations card =
        locations
        |> List.tryFind (fitsInLocation locCardMap card)

    let getSortByValue setData (entryWithInfo: CardEntryWithInfo) sortBy =
        let entry = entryWithInfo.data
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

        let sortBy (agedEntryWithInfo: AgedCardEntryWithInfo) =
            let entryWithInfo =
                WithInfo.map OldAmountable.data agedEntryWithInfo

            sortRules
            |> List.map (getSortByValue setData entryWithInfo)
            // We add a random factor at the end
            |> (fun lst -> List.append lst [ random.Next(0, 10000) |> sprintf "%04i" ])

        entries |> List.sortBy sortBy

    let take (setData: SetDataMap) (infoMap: CardInfoMap) locations (entries: AgedCardEntry list) =
        let mutable locCardMap : Map<InventoryLocation, AgedCardWithInfo list> =
            let mutable map = Map.empty
            map <- Map.add Fallback [] map

            for location in locations do
                map <- Map.add (Custom location) [] map

            map

        // We sort the cards before trying to put them into locations
        let random = Random()

        let agedEntriesWithInfo =
            entries
            // We can consider a card only for inventory, if we have the info
            |> List.choose
                (fun (agedEntry: AgedCardEntry) ->
                    let entry = agedEntry.data

                    Map.tryFind (entry.card.set, entry.card.number) infoMap
                    |> Option.map (fun info -> AgedCardEntryWithInfo.create info agedEntry))
            // TODO: generalize and make configurable
            |> List.sortBy
                (fun oldAmountable ->
                    let entryWithInfo = oldAmountable.data

                    [
                      // Language
                      match entryWithInfo.data.card.language.Value with
                      | "en" -> "0"
                      | "de" -> "1"
                      | _ -> "2"
                      // Foil
                      if entryWithInfo.data.card.foil then
                          "0"
                      else
                          "1"
                      // Set
                      (Map.find entryWithInfo.data.card.set setData)
                          .date
                      entryWithInfo.data.card.number.Value
                      // Random
                      random.Next(0, 10000) |> string ])

        for agedEntryWithInfo in agedEntriesWithInfo do
            let getCard old =
                WithInfo.map
                    (fun (agedEntry: AgedCardEntry) -> AgedCard.create old agedEntry.data.card)
                    agedEntryWithInfo

            let old = getCard true
            let notOld = getCard false

            // for .. to does not work for uint
            // BEWARE: Very unfunctional code ahead!
            let mutable i = 0u

            while i < agedEntryWithInfo.data.data.amount do
                let agedCard =
                    if i < agedEntryWithInfo.data.amountOld then
                        old
                    else
                        notOld

                let location =
                    WithInfo.map Oldable.data agedCard
                    |> determineLocation locCardMap locations

                match location with
                | Some location ->
                    locCardMap <- Map.change (Custom location) (Option.map (fun l -> agedCard :: l)) locCardMap
                | None -> locCardMap <- Map.change Fallback (Option.map (fun l -> agedCard :: l)) locCardMap

                i <- i + 1u

        // Collapse into entries again and sort by location rules
        locCardMap
        |> Map.map
            (fun location cardList ->
                cardList
                |> AgedCardEntryWithInfo.fromCardList
                |> sortEntries setData location
                |> List.map WithInfo.data)

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
