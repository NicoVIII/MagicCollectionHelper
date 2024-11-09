namespace MagicCollectionHelper.Core

open SimpleOptics

[<AutoOpen>]
module DomainTypesModules =
    [<RequireQualifiedAccess>]
    module NumBase =
        let toString (numBase: NumBase) =
            match numBase with
            | Decimal -> "Decimal"
            | Dozenal -> "Dozenal"
            | Seximal -> "Seximal"

    [<RequireQualifiedAccess>]
    module DeckStatsCardEntry =
        let toEntry cardInfoMap (entry: DeckStatsCardEntry) =
            match entry.set, entry.language with
            | Some set, Some lang ->
                match set, entry.number with
                // Special case for "The List", because numbers differ between scryfall and deckstats
                | MagicSet "PLST" as set, _
                | set, None ->
                    // We try to determine the number with name and set
                    cardInfoMap
                    |> Map.tryFind (entry.name, set)
                    |> Option.map (fun info ->
                        Card.create entry.foil lang info.collectorNumber set
                        |> Entry.create entry.amount)
                | set, Some number -> Card.create entry.foil lang number set |> Entry.create entry.amount |> Some
            | _ -> None // We need set and lang as minimum

        let listToEntries cardInfoMap (entries: DeckStatsCardEntry list) =
            // We change the map to improve lookup perf
            let cardInfoMap =
                cardInfoMap
                |> Map.toList
                |> List.map snd
                |> List.groupBy (fun (info: CardInfo) -> (info.name, info.set))
                |> List.choose (fun (key, infoList) ->
                    match infoList with
                    | [ info ] -> (key, info) |> Some
                    // Kein (eindeutiges) Ergebnis gefunden
                    | _ -> None)
                |> Map.ofList

            entries
            // We only take entries with enough info
            |> List.fold
                (fun lst (entry: DeckStatsCardEntry) ->
                    match toEntry cardInfoMap entry with
                    | Some entry -> entry :: lst
                    | None -> lst)
                []
            // We remove now all duplicates
            |> List.groupBy (fun entry -> entry.card)
            |> List.map (fun (card, entryList) -> {
                amount = List.sumBy (Optic.get EntryOptic.amount) entryList
                card = card
            })
            |> List.rev

        let listToEntriesAsync cardInfoMap (entries: DeckStatsCardEntry list) =
            async { return listToEntries cardInfoMap entries }

    [<RequireQualifiedAccess>]
    module Analyser =
        let create emptyData collect postprocess print = {
            emptyData = emptyData
            collect = collect
            postprocess = postprocess
            print = print
        }

    [<RequireQualifiedAccess>]
    module Prefs =
        let create cardGroupMinSize cardGroupMaxSize numBase missingPercent setWithFoils : Prefs = {
            cardGroupMinSize = cardGroupMinSize
            cardGroupMaxSize = cardGroupMaxSize
            numBase = numBase
        }

    [<RequireQualifiedAccess>]
    module Rules =
        let empty = {
            inSet = None
            inLanguage = None
            isFoil = None
            isToken = None
            typeContains = None
            typeNotContains = None
            limit = None
            limitExact = None
            rarity = None
            colorIdentity = None
        }

        let withInSet v rules = { rules with inSet = Some v }
        let withInLanguage v rules = { rules with inLanguage = Some v }
        let withIsFoil v rules = { rules with isFoil = Some v }
        let withIsToken v rules = { rules with isToken = Some v }
        let withTypeContains v rules = { rules with typeContains = Some v }
        let withTypeNotContains v rules = { rules with typeNotContains = Some v }
        let withLimit v rules = { rules with limit = Some v }
        let withLimitExact v rules = { rules with limitExact = Some v }
        let withRarity v rules : Rules = { rules with rarity = Some v }
        let withColorIdentity v rules : Rules = { rules with colorIdentity = Some v }
