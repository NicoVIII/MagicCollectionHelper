namespace MagicCollectionHelper.Core.Types

open Myriad.Plugins

open MagicCollectionHelper.Core.Types.Generated

/// Type which holds user preferences so the user can customize some behaviors
[<Generator.Lenses("types", "Lens")>]
type Prefs =
    { dozenalize: bool
      missingPercent: float
      setWithFoils: bool }

module Prefs =
    let create dozenalize missingPercent setWithFoils =
        { dozenalize = dozenalize
          missingPercent = missingPercent
          setWithFoils = setWithFoils }

// TODO: condition
// TODO: comment?
// TODO: pinned?
// TODO: added
type DeckStatsCardEntry =
    { amount: uint
      name: string
      number: CollectorNumber option
      foil: bool
      language: Language option
      set: MagicSet option }

module DeckStatsCardEntry =
    let toEntry cardInfoMap (entry: DeckStatsCardEntry) =
        match entry.set, entry.number, entry.language with
        | Some set, Some number, Some lang ->
            Card.create entry.foil lang number set
            |> CardEntry.create entry.amount
            |> Some
        // We try to determine the number with name and set
        | Some set, None, Some lang ->
            cardInfoMap
            |> Map.tryFind (entry.name, set)
            |> Option.map
                (fun info ->
                    Card.create entry.foil lang info.collectorNumber set
                    |> CardEntry.create entry.amount)
        | _ -> None

    let listToEntries cardInfoMap (entries: DeckStatsCardEntry list) =
        // We change the map to improve lookup perf
        let cardInfoMap =
            cardInfoMap
            |> Map.toList
            |> List.map snd
            |> List.groupBy (fun (info: CardInfo) -> (info.name, info.set))
            |> List.choose
                (fun (key, infoList) ->
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
        |> List.map
            (fun (card, entryList) ->
                { amount = List.sumBy (fun (entry: CardEntry) -> entry.amount) entryList
                  card = card })
        |> List.rev

    let listToEntriesAsync cardInfoMap (entries: DeckStatsCardEntry list) =
        async { return listToEntries cardInfoMap entries }

type SetData =
    { date: string
      max: uint
      name: string }

type SetDataMap = Map<MagicSet, SetData>

type Analyser<'result, 'collect, 'settings> =
    { emptyData: (unit -> 'collect)
      collect: ('settings -> 'collect -> DeckStatsCardEntry -> 'collect)
      postprocess: (SetDataMap -> 'collect -> 'result)
      print: ('settings -> 'result -> string seq) }

module Analyser =
    let create emptyData collect postprocess print =
        { emptyData = emptyData
          collect = collect
          postprocess = postprocess
          print = print }

type SortRule =
    | ByColorIdentity
    | BySet
    | ByCollectorNumber
    | ByName
    | ByCmc
    | ByTypeContains of string list
    | ByRarity of Set<Rarity> list
    | ByLanguage of Language list

type SortRules = SortRule list

type CustomLocationName = string

type Rules =
    { inSet: Set<MagicSet> option
      inLanguage: Language option
      isFoil: bool option
      isToken: bool option
      typeContains: Set<string> option
      typeNotContains: Set<string> option
      limit: uint option
      limitExact: uint option
      rarity: Set<Rarity> option
      colorIdentity: Set<ColorIdentity> option }

module Rules =
    let empty =
        { inSet = None
          inLanguage = None
          isFoil = None
          isToken = None
          typeContains = None
          typeNotContains = None
          limit = None
          limitExact = None
          rarity = None
          colorIdentity = None }

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

[<Generator.Lenses("types", "Lens")>]
type CustomLocation =
    { name: CustomLocationName
      rules: Rules
      sortBy: SortRules }

/// Location, where a part of the collection is
type InventoryLocation =
    | Custom of CustomLocation
    | Fallback

type LocationWithCards = (InventoryLocation * OldAmountable<CardEntry> list) list
