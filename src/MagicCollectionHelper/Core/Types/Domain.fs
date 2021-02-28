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

type CollectorNumber =
    | CollectorNumber of uint32
    static member unwrap(CollectorNumber x) = x
    member this.Value = CollectorNumber.unwrap this

// TODO: Provide additional data for set and Language through external file?
// TODO: So a user could add it, if it is missing in the application itself
type MagicSet =
    private | MagicSet of string
    member this.Value =
        let (MagicSet value) = this
        value

module MagicSet =
    // New abbreviatens taken from https://mtg.gamepedia.com/Set#List_of_Magic_expansions_and_sets
    let convertSetAbbrev =
        function
        | "1E" -> "LEA" // Alpha (Limited Edition)
        | "2E" -> "LEB" // Beta (Limited Edition)
        | "2U" -> "2ED" // Unlimited Edition
        | "AN" -> "ARN" // Arabian Nights
        | "AQ" -> "ATQ" // Antiquities
        | "3E" -> "3ED" // Revised Edition
        | "LE" -> "LEG" // Legends
        | "DK" -> "DRK" // The Dark
        | "FE" -> "FEM" // Fallen Empires
        | "4E" -> "4ED" // Fourth Edition
        | "IA" -> "ICE" // Ice Age
        | "CH" -> "CHR" // Chronicles
        | "HM" -> "HML" // Homelands
        | "AL" -> "ALL" // Alliances
        | "MI" -> "MIR" // Mirage
        | "VI" -> "VIS" // Visions
        | "5E" -> "5ED" // Fifth Edition
        | "PO" -> "POR" // Portal
        | "WL" -> "WTH" // Weatherlight
        | "TE" -> "TMP" // Tempest
        | "ST" -> "STH" // Stronghold
        | "EX" -> "EXO" // Exodus
        | "P2" -> "P02" // Portal Second Age
        | "UG" -> "UGL" // Unglued
        | "UZ" -> "USG" // Urza's Saga
        | "UL" -> "ULG" // Urza's Legacy
        | "6E" -> "6ED" // Sixth Edition
        | "PK" -> "PTK" // Portal Three Kingdoms
        | "UD" -> "UDS" // Urza's Destiny
        | "P3" -> "S99" // Starter 1999
        | "MM" -> "MMQ" // Mercadian Masques
        | "NE" -> "NEM" // Nemesis
        | "PR" -> "PCY" // Prophecy
        | "IN" -> "INV" // Invasion
        | "PS" -> "PLS" // Planeshift
        | "7E" -> "7ED" // Seventh Edition
        | "AP" -> "APC" // Apocalypse
        | "OD" -> "ODY" // Odyssey
        | set -> set

    let create =
        (fun (s: string) -> s.ToUpper())
        >> convertSetAbbrev
        >> MagicSet

type Language =
    | Language of string
    member this.Value =
        let (Language value) = this
        value

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

type Card =
    { number: CollectorNumber
      foil: bool
      language: Language
      set: MagicSet }

type CardEntry =
    { amount: uint
      number: CollectorNumber
      foil: bool
      language: Language
      set: MagicSet }

module CardEntry =
    let listFromDeckStats (entries: DeckStatsCardEntry list) =
        entries
        // We only take entries with enough info
        |> List.fold (fun lst (entry: DeckStatsCardEntry) ->
            match entry.set, entry.number, entry.language with
            | Some set, Some number, Some lang ->
                { CardEntry.amount = entry.amount
                  foil = entry.foil
                  set = set
                  number = number
                  language = lang
                } :: lst
            | _ -> lst
        ) []
        |> List.rev

[<Generator.DuCases("dus")>]
type Rule =
    | InSet of MagicSet
    | InLanguage of Language
    | IsFoil of bool
    | Limit of uint

type CustomLocation =
    { name: string
      rules: Rule list }

/// Location, where a part of the collection is
type InventoryLocation =
    | Custom of CustomLocation
    | Fallback

type Color =
    | White
    | Blue
    | Black
    | Red
    | Green

type CardInfo = {
    name: string
    set: MagicSet
    collectorNumber: CollectorNumber
    colors: Color list
    colorIdentity: Color list
}

type CardInfoMap = Map<MagicSet * CollectorNumber, CardInfo>
