//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.Core.Types.Generated

module CardWithInfo =
    open MagicCollectionHelper.Core.Types
    let card (x: CardWithInfo) = x.card
    let info (x: CardWithInfo) = x.info
    let create (card: Card) (info: CardInfo): CardWithInfo = { card = card; info = info }
    let map (mapcard: Card -> Card) (mapinfo: CardInfo -> CardInfo) (record': CardWithInfo) =
        { record' with
              card = mapcard record'.card
              info = mapinfo record'.info }
namespace rec MagicCollectionHelper.Core.Types.Generated

module Rarity =
    open MagicCollectionHelper.Core.Types
    let toString (x: Rarity) =
        match x with
        | Common -> "Common"
        | Uncommon -> "Uncommon"
        | Rare -> "Rare"
        | Mythic -> "Mythic"

    let fromString (x: string) =
        match x with
        | "Common" -> Some Common
        | "Uncommon" -> Some Uncommon
        | "Rare" -> Some Rare
        | "Mythic" -> Some Mythic
        | _ -> None

    let toTag (x: Rarity) =
        match x with
        | Common -> 0
        | Uncommon -> 1
        | Rare -> 2
        | Mythic -> 3

    let isCommon (x: Rarity) =
        match x with
        | Common -> true
        | _ -> false

    let isUncommon (x: Rarity) =
        match x with
        | Uncommon -> true
        | _ -> false

    let isRare (x: Rarity) =
        match x with
        | Rare -> true
        | _ -> false

    let isMythic (x: Rarity) =
        match x with
        | Mythic -> true
        | _ -> false
