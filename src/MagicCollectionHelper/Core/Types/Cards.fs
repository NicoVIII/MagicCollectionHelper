namespace MagicCollectionHelper.Core

open Myriad.Plugins

[<AutoOpen>]
module CardTypes =
    /// Makes it possible to mark some type as old or not old
    type Oldable<'a> = { data: 'a; old: bool }

    /// Makes it possible to add an oldAmount to a type
    type OldAmountable<'a> = { amountOld: uint; data: 'a }

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
        static member unwrap(Language x) = x
        member this.Value = Language.unwrap this

    /// The number for collectors of a card inside a given set
    type CollectorNumber =
        | CollectorNumber of string
        static member unwrap(CollectorNumber x) = x
        member this.Value = CollectorNumber.unwrap this

    /// The identifier of a magic set
    type MagicSet =
        | MagicSet of string
        static member unwrap(MagicSet x) = x
        member this.Value = MagicSet.unwrap this

    type ColorIdentity = Set<Color>

    [<Generator.DuCases("core")>]
    type Rarity =
        | Common
        | Uncommon
        | Rare
        | Mythic
        | Special
        | Bonus

    /// Additional info for a card
    [<Generator.Fields("core")>]
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
    [<Generator.Fields("core")>]
    type Card =
        { foil: bool
          language: Language
          number: CollectorNumber
          set: MagicSet }

    /// A card entry, which is used to condense multiple cards into one card object and an amount
    [<Generator.Fields("core")>]
    type CardEntry = { amount: uint; card: Card }

    [<Generator.Fields("core")>]
    type CardWithInfo = { card: Card; info: CardInfo }

    [<Generator.Fields("core")>]
    type CardEntryWithInfo = { entry: CardEntry; info: CardInfo }
