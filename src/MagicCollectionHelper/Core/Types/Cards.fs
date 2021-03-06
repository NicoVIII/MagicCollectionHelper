namespace MagicCollectionHelper.Core.Types

/// The number for collectors of a card inside a given set
type CollectorNumber =
    | CollectorNumber of uint32
    static member unwrap(CollectorNumber x) = x
    member this.Value = CollectorNumber.unwrap this

// TODO: Provide additional data for set and Language through external file?
// TODO: So a user could add it, if it is missing in the application itself
/// The identifier of a magic set
type MagicSet =
    private
    | MagicSet of string
    member this.Value =
        let (MagicSet value) = this
        value

module MagicSet =
    /// Converts some old two letter set abbreviations to the new three letter counterpart
    /// New abbreviatens taken from https://mtg.gamepedia.com/Set#List_of_Magic_expansions_and_sets
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

/// The identifier for a language, typically the language a card is in
type Language =
    | Language of string
    member this.Value =
        let (Language value) = this
        value

/// A card identified by as few properties as possible
type Card =
    { number: CollectorNumber
      foil: bool
      language: Language
      set: MagicSet }

/// A card entry, which is used to condense multiple cards into one card object and an amount
type CardEntry = { card: Card; amount: uint }

/// All colors which are important in Magic
type Color =
    | White
    | Blue
    | Black
    | Red
    | Green

/// Additional info for a card
type CardInfo =
    { name: string
      set: MagicSet
      collectorNumber: CollectorNumber
      colors: Color list
      colorIdentity: Color list
      oracleId: string }

type CardInfoMap = Map<MagicSet * CollectorNumber, CardInfo>
