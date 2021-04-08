namespace MagicCollectionHelper.Core.Types

open Myriad.Plugins

/// All colors which are important in Magic
type Color =
    | White
    | Blue
    | Black
    | Red
    | Green

/// The identifier for a language, typically the language a card is in
type Language =
    | Language of string
    member this.Value =
        let (Language value) = this
        value

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
        // Deckstats has strange abbreviations I fix here
        | "GU" -> "ULG" // Urza's Legacy
        | set -> set

    let create =
        (fun (s: string) -> s.ToUpper())
        >> convertSetAbbrev
        >> MagicSet

type ColorIdentity = Set<Color>

module ColorIdentity =
    let colorless = Set.empty

    // Mono
    let white = White |> Set.singleton
    let blue = Blue |> Set.singleton
    let black = Black |> Set.singleton
    let red = Red |> Set.singleton
    let green = Green |> Set.singleton

    // Two colors - Ravnica guilds
    let azorius = [ White; Blue ] |> Set.ofList
    let dimir = [ Blue; Black ] |> Set.ofList
    let rakdos = [ Black; Red ] |> Set.ofList
    let gruul = [ Red; Green ] |> Set.ofList
    let selesnya = [ Green; White ] |> Set.ofList
    let orzhov = [ White; Black ] |> Set.ofList
    let izzet = [ Blue; Red ] |> Set.ofList
    let golgari = [ Black; Green ] |> Set.ofList
    let boros = [ Red; White ] |> Set.ofList
    let simic = [ Green; Blue ] |> Set.ofList

    // Three colors - shards of alara + div.
    let jund = [ Red; Green; Black ] |> Set.ofList
    let bant = [ Green; White; Blue ] |> Set.ofList
    let grixis = [ Blue; Black; Red ] |> Set.ofList
    let naya = [ Red; Green; White ] |> Set.ofList
    let esper = [ White; Blue; Black ] |> Set.ofList
    let jeskai = [ Blue; Red; White ] |> Set.ofList
    let mardu = [ Red; White; Black ] |> Set.ofList
    let sultai = [ Black; Green; Blue ] |> Set.ofList
    let temur = [ Green; Blue; Red ] |> Set.ofList
    let abzan = [ White; Black; Green ] |> Set.ofList

    // Four colors - shards of alara + div.
    let nonWhite =
        [ Blue; Black; Red; Green ] |> Set.ofList

    let nonBlue =
        [ Black; Red; Green; White ] |> Set.ofList

    let nonBlack =
        [ Red; Green; White; Blue ] |> Set.ofList

    let nonRed =
        [ Green; White; Blue; Black ] |> Set.ofList

    let nonGreen =
        [ White; Blue; Black; Red ] |> Set.ofList

    // Five colors
    let allColors =
        [ White; Blue; Black; Red; Green ] |> Set.ofList

    let private sorted =
        [ colorless
          white
          blue
          black
          red
          green
          azorius
          dimir
          rakdos
          gruul
          selesnya
          orzhov
          izzet
          golgari
          boros
          simic
          jund
          bant
          grixis
          naya
          esper
          jeskai
          mardu
          sultai
          temur
          abzan
          nonWhite
          nonBlue
          nonBlack
          nonRed
          nonGreen
          allColors ]

    let private posMap =
        sorted
        |> List.indexed
        |> List.map (fun (index, colorIdentity) -> (colorIdentity, index))
        |> Map.ofList

    let getPosition (colorIdentity: ColorIdentity) = posMap.Item colorIdentity

[<Generator.DuCases("cards")>]
type Rarity =
    | Common
    | Uncommon
    | Rare
    | Mythic

/// Additional info for a card
[<Generator.Fields("cards")>]
type CardInfo =
    { name: string
      set: MagicSet
      collectorNumber: CollectorNumber
      colors: Set<Color>
      colorIdentity: ColorIdentity
      oracleId: string
      rarity: Rarity
      typeLine: string
      cmc: uint }

type CardInfoMap = Map<MagicSet * CollectorNumber, CardInfo>

/// A card identified by as few properties as possible
[<Generator.Fields("cards")>]
type Card =
    { number: CollectorNumber
      foil: bool
      language: Language
      set: MagicSet }

module Card =
    let isExactSame (card1: Card) (card2: Card) =
        card1.set = card2.set
        && card1.number = card2.number

/// A card entry, which is used to condense multiple cards into one card object and an amount
[<Generator.Fields("cards")>]
type CardEntry = { amount: uint; card: Card }

module CardEntry =
    let collapseCardList cardList =
        cardList
        |> List.fold
            (fun cardAmountMap card ->
                Map.change
                    card
                    (function
                    | Some amount -> amount + 1u |> Some
                    | None -> Some 1u)
                    cardAmountMap)
            Map.empty
        |> Map.toList
        |> List.map (fun (card, amount) -> { card = card; amount = amount })

[<Generator.Fields("cards")>]
type CardWithInfo = { card: Card; info: CardInfo }

module CardWithInfo =
    let create card info = { card = card; info = info }

    /// Checks if those two cards are rulewise the same. They do not have to be from the same set
    let isSame (card1: CardWithInfo) (card2: CardWithInfo) =
        card1.info.oracleId = card2.info.oracleId

    let isExactSame (card1: CardWithInfo) (card2: CardWithInfo) = Card.isExactSame card1.card card2.card

[<Generator.Fields("cards")>]
type CardEntryWithInfo = { entry: CardEntry; info: CardInfo }

module CardEntryWithInfo =
    let create entry info = { entry = entry; info = info }
