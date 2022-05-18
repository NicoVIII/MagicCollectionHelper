namespace MagicCollectionHelper.Core

open SimpleOptics
open System

[<RequireQualifiedAccess>]
module Inventory =
    module Rules =
        let fitsRule option rule =
            match option with
            | None -> true
            | Some value -> rule value

        let fitsInSetRule cardWithInfo rules =
            let set = cardWithInfo ^. CardWithInfoOptic.set
            fitsRule rules.inSet (Set.contains set)

        let fitsInLanguageRule cardWithInfo rules =
            let language = cardWithInfo ^. CardWithInfoOptic.language

            fitsRule rules.inLanguage (fun language -> language = language)

        let fitsIsFoil cardWithInfo rules =
            let foil = cardWithInfo ^. CardWithInfoOptic.foil

            fitsRule rules.isFoil (fun shouldBeFoil -> shouldBeFoil = foil)

        let fitsIsToken cardWithInfo rules =
            let card = cardWithInfo ^. CardWithInfoOptic.card

            fitsRule rules.isToken (fun shouldBeToken -> shouldBeToken = Card.isToken card)

        let fitsTypeContains cardWithInfo rules =
            let info = cardWithInfo ^. CardWithInfoOptic.info

            fitsRule rules.typeContains (Set.forall info.typeLine.Contains)

        let fitsTypeNotContains cardWithInfo rules =
            let info = cardWithInfo ^. CardWithInfoOptic.info

            fitsRule rules.typeNotContains (Set.forall (info.typeLine.Contains >> not))

        let fitsRarity cardWithInfo rules =
            let rarity = cardWithInfo ^. CardWithInfoOptic.rarity

            fitsRule rules.rarity (fun rarities -> Set.contains rarity rarities)

        let fitsColorIdentity cardWithInfo rules =
            let colorIdentity = cardWithInfo ^. CardWithInfoOptic.colorIdentity

            fitsRule rules.colorIdentity (fun colorIdentities -> Set.contains colorIdentity colorIdentities)

        let fitsLimit cardsInLoc cardWithInfo rules =
            let rule limit =
                let sum =
                    List.sumBy
                        (fun c ->
                            if CardWithInfo.isSame c cardWithInfo then
                                1
                            else
                                0)
                        cardsInLoc

                (uint) sum < limit

            fitsRule rules.limit rule

        let fitsLimitExact cardsInLoc cardWithInfo rules =
            let rule limitExact =
                let sum =
                    List.sumBy
                        (fun c ->
                            if CardWithInfo.isExactSame c cardWithInfo then
                                1
                            else
                                0)
                        cardsInLoc

                (uint) sum < limitExact

            fitsRule rules.limitExact rule

        let fitsAll cardsInLoc cardWithInfo rules =
            [
                fitsInSetRule
                fitsInLanguageRule
                fitsIsFoil
                fitsIsToken
                fitsTypeContains
                fitsTypeNotContains
                fitsColorIdentity
                fitsRarity
                fitsLimit cardsInLoc
                fitsLimitExact cardsInLoc
            ]
            // Evaluate functions
            |> List.map (fun fnc -> fnc cardWithInfo rules)
            |> List.forall id

    let fitsInLocation (locCardMap: Map<InventoryLocation, AgedCardWithInfo list>) card location =
        let cardList =
            locCardMap.Item(Custom location)
            |> List.map (WithInfo.map (Optic.get AgedCardOptic.card))

        Rules.fitsAll cardList card location.rules

    let determineLocation locCardMap locations card =
        locations
        |> List.tryFind (fitsInLocation locCardMap card)

    let getSortByValue setData entryWithInfo sortBy =
        match sortBy with
        | ByColorIdentity ->
            let pos =
                entryWithInfo ^. EntryWithInfoOptic.colorIdentity
                |> ColorIdentity.getPosition

            sprintf "%02i" pos
        | ByName -> entryWithInfo ^. EntryWithInfoOptic.name
        | BySet ->
            let set = entryWithInfo ^. EntryWithInfoOptic.set
            let setValue = set |> MagicSet.unwrap

            let date =
                Map.tryFind set setData
                |> function
                    | Some setData -> setData.date
                    | None -> "0000-00-00"

            let extension =
                match setValue with
                | set when set.StartsWith "T" -> set.Substring 1 + "Z"
                | set -> set + "A"

            $"{date}{extension}"
        | ByCollectorNumber ->
            (entryWithInfo ^. EntryWithInfoOptic.number
             |> CollectorNumber.unwrap)
                .PadLeft(3, '0')
            |> sprintf "%s"
        | ByCmc ->
            entryWithInfo ^. EntryWithInfoOptic.cmc
            |> sprintf "%02i"
        | ByTypeContains typeContains ->
            let typeLine = entryWithInfo ^. EntryWithInfoOptic.typeLine

            typeContains
            |> List.fold
                (fun (found, strng) typeContains ->
                    if found then
                        (true, strng + "9")
                    else if typeLine.Contains typeContains then
                        (true, strng + "1")
                    else
                        (false, strng + "9"))
                (false, "")
            |> snd
        | ByRarity rarities ->
            let rarity = entryWithInfo ^. EntryWithInfoOptic.rarity

            rarities
            |> List.indexed
            |> List.tryPick (fun (index, raritySet) ->
                if Set.contains rarity raritySet then
                    Some index
                else
                    None)
            |> Option.defaultValue (List.length rarities)
            |> string
        | ByLanguage languages ->
            let language = entryWithInfo ^. EntryWithInfoOptic.language

            languages
            |> List.indexed
            |> List.tryPick (fun (index, sLanguage) ->
                if sLanguage = language then
                    Some index
                else
                    None)
            |> Option.defaultValue (List.length languages)
            |> string

    let sortEntries setData location entries =
        let random = Random()

        let sortRules =
            match location with
            | Custom location -> location.sortBy
            | Fallback -> [ ByName ]

        let sortBy (agedEntryWithInfo: AgedEntryWithInfo) =
            let entryWithInfo = agedEntryWithInfo |> AgedEntryWithInfo.removeAge

            sortRules
            |> List.map (getSortByValue setData entryWithInfo)
            // We add a random factor at the end
            |> (fun lst ->
                List.append
                    lst
                    [
                        random.Next(0, 10000) |> sprintf "%04i"
                    ])

        entries |> List.sortBy sortBy

    let take (setData: SetDataMap) (infoMap: CardInfoMap) locations (entries: AgedEntry list) =
        let mutable locCardMap: Map<InventoryLocation, AgedCardWithInfo list> =
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
            |> List.choose (AgedEntryWithInfo.fromEntry infoMap)
            // TODO: generalize and make configurable
            |> List.sortBy (fun entry ->
                let foil = entry ^. AgedEntryWithInfoOptic.foil

                let language =
                    entry ^. AgedEntryWithInfoOptic.language
                    |> Language.unwrap

                let number =
                    entry ^. AgedEntryWithInfoOptic.number
                    |> CollectorNumber.unwrap

                let set = entry ^. AgedEntryWithInfoOptic.set

                [ // Language
                    match language with
                    | "en" -> "0"
                    | "de" -> "1"
                    | _ -> "2"
                    // Foil
                    if foil then "0" else "1"
                    // Set
                    (Map.find set setData).date
                    number
                    // Random
                    random.Next(0, 10000) |> string
                ])

        for agedEntryWithInfo in agedEntriesWithInfo do
            let getCard old =
                WithInfo.map
                    (fun agedEntry ->
                        agedEntry ^. AgedEntryOptic.card
                        |> AgedCard.create old)
                    agedEntryWithInfo

            let old = getCard true
            let notOld = getCard false

            // for .. to does not work for uint
            // BEWARE: Very unfunctional code ahead!
            let mutable i = 0u

            while i < agedEntryWithInfo.data.data.amount do
                let card =
                    if i < agedEntryWithInfo.data.amountOld then
                        old
                    else
                        notOld

                let location =
                    card
                    |> AgedCardWithInfo.removeAge
                    |> determineLocation locCardMap locations

                match location with
                | Some location ->
                    locCardMap <- Map.change (Custom location) (Option.map (fun l -> card :: l)) locCardMap
                | None -> locCardMap <- Map.change Fallback (Option.map (fun l -> card :: l)) locCardMap

                i <- i + 1u

        // Collapse into entries again and sort by location rules
        locCardMap
        |> Map.map (fun location cardList ->
            cardList
            |> AgedEntryWithInfo.fromCardList
            |> sortEntries setData location
            |> List.map (Optic.get AgedEntryWithInfoOptic.agedEntry))
        // If the Fallback is empty, we don't need it
        |> Map.change Fallback (function
            | None
            | Some [] -> None
            | Some lst -> Some lst)
        // We now convert the map back into a list (order matters!) and sort it
        |> Map.toList
        |> List.sortBy (fun (location, _) ->
            match location with
            | Fallback -> 9999
            | Custom location -> List.findIndex (fun l -> location = l) locations)

    // Because this process can take some time, we provide an async version
    let takeAsync setData infoMap locations entries =
        async { return take setData infoMap locations entries }
