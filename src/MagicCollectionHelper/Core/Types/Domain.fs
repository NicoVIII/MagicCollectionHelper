namespace MagicCollectionHelper.Core.Types

open Myriad.Plugins

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
      number: CollectorNumber option
      foil: bool
      language: Language option
      set: MagicSet option }

module DeckStatsCardEntry =
    let listToEntries (entries: DeckStatsCardEntry list) =
        entries
        // We only take entries with enough info
        |> List.fold
            (fun lst (entry: DeckStatsCardEntry) ->
                match entry.set, entry.number, entry.language with
                | Some set, Some number, Some lang ->
                    { CardEntry.amount = entry.amount
                      card =
                          { foil = entry.foil
                            set = set
                            number = number
                            language = lang } }
                    :: lst
                | _ -> lst)
            []
        |> List.rev

type SetData =
    { date: string
      max: CollectorNumber
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
    | BySet
    | ByCollectorNumber
    | ByName

type SortRules = SortRule list

type CustomLocationName = string

type Rules =
    { inSet: Set<MagicSet> option
      inLanguage: Language option
      isFoil: bool option
      limit: uint option
      limitExact: uint option
      colorIdentity: Set<ColorIdentity> option }

module Rules =
    let createEmpty () =
        { inSet = None
          inLanguage = None
          isFoil = None
          limit = None
          limitExact = None
          colorIdentity = None }

    let withInSet v rules = { rules with inSet = Some v }
    let withInLanguage v rules = { rules with inLanguage = Some v }
    let withIsFoil v rules = { rules with isFoil = Some v }
    let withLimit v rules = { rules with limit = Some v }
    let withLimitExact v rules = { rules with limitExact = Some v }
    let withColorIdentity v rules : Rules = { rules with colorIdentity = Some v }

type RawCustomLocation =
    { name: CustomLocationName
      rules: Rules
      sortBy: SortRules }

[<Generator.Lenses("types", "Lens")>]
type CustomLocation =
    { name: CustomLocationName
      rules: Rules
      sortBy: SortRules
      position: uint }

module CustomLocation =
    let createFromRaw pos (raw: RawCustomLocation) =
        { CustomLocation.name = raw.name
          rules = raw.rules
          sortBy = raw.sortBy
          position = pos }

/// Location, where a part of the collection is
type InventoryLocation =
    | Custom of CustomLocation
    | Fallback
