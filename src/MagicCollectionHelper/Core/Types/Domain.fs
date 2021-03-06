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

[<Generator.DuCases("dus")>]
type Rule =
    | InSet of MagicSet
    | InLanguage of Language
    | IsFoil of bool
    | Limit of uint

type CustomLocation = { name: string; rules: Rule list }

/// Location, where a part of the collection is
type InventoryLocation =
    | Custom of CustomLocation
    | Fallback
