namespace MagicCollectionHelper.Core

open Myriad.Plugins

[<AutoOpen>]
module CardTypes =
    /// A card identified by as few properties as possible
    [<Generator.Fields("core")>]
    [<Generator.Lenses("core", "SimpleOptics.Lens")>]
    type Card =
        { foil: bool
          language: Language
          number: CollectorNumber
          set: MagicSet }

    /// A card entry, which is used to condense multiple cards into one card object and an amount
    [<Generator.Fields("core")>]
    [<Generator.Lenses("core", "SimpleOptics.Lens")>]
    type Entry = { amount: uint; card: Card }

    /// Additional info for a card
    [<Generator.Fields("core")>]
    [<Generator.Lenses("core", "SimpleOptics.Lens")>]
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

    /// Makes it possible to mark some type as old or not old
    type Oldable<'a> = { data: 'a; old: bool }

    /// Makes it possible to add an oldAmount to a type
    type OldAmountable<'a> = { amountOld: uint; data: 'a }

    type WithInfo<'a> = { data: 'a; info: CardInfo }

    type CardWithInfo = WithInfo<Card>
    type EntryWithInfo = WithInfo<Entry>
    type AgedCard = Oldable<Card>
    type AgedEntry = OldAmountable<Entry>
    type AgedCardWithInfo = WithInfo<AgedCard>
    type AgedEntryWithInfo = WithInfo<AgedEntry>
