//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.Core

module CardLenses =
    open MagicCollectionHelper.Core.CardTypes
    let foil =
        Lens((fun (x: Card) -> x.foil), (fun (x: Card) (value: bool) -> { x with foil = value }))

    let language =
        Lens((fun (x: Card) -> x.language), (fun (x: Card) (value: Language) -> { x with language = value }))

    let number =
        Lens((fun (x: Card) -> x.number), (fun (x: Card) (value: CollectorNumber) -> { x with number = value }))

    let set =
        Lens((fun (x: Card) -> x.set), (fun (x: Card) (value: MagicSet) -> { x with set = value }))
namespace rec MagicCollectionHelper.Core

module EntryLenses =
    open MagicCollectionHelper.Core.CardTypes
    let amount =
        Lens((fun (x: Entry) -> x.amount), (fun (x: Entry) (value: uint) -> { x with amount = value }))

    let card =
        Lens((fun (x: Entry) -> x.card), (fun (x: Entry) (value: Card) -> { x with card = value }))
namespace rec MagicCollectionHelper.Core

module CardInfoLenses =
    open MagicCollectionHelper.Core.CardTypes
    let name =
        Lens((fun (x: CardInfo) -> x.name), (fun (x: CardInfo) (value: string) -> { x with name = value }))

    let set =
        Lens((fun (x: CardInfo) -> x.set), (fun (x: CardInfo) (value: MagicSet) -> { x with set = value }))

    let collectorNumber =
        Lens(
            (fun (x: CardInfo) -> x.collectorNumber),
            (fun (x: CardInfo) (value: CollectorNumber) -> { x with collectorNumber = value })
        )

    let colors =
        Lens((fun (x: CardInfo) -> x.colors), (fun (x: CardInfo) (value: Set<Color>) -> { x with colors = value }))

    let colorIdentity =
        Lens(
            (fun (x: CardInfo) -> x.colorIdentity),
            (fun (x: CardInfo) (value: ColorIdentity) -> { x with colorIdentity = value })
        )

    let oracleId =
        Lens((fun (x: CardInfo) -> x.oracleId), (fun (x: CardInfo) (value: string) -> { x with oracleId = value }))

    let rarity =
        Lens((fun (x: CardInfo) -> x.rarity), (fun (x: CardInfo) (value: Rarity) -> { x with rarity = value }))

    let typeLine =
        Lens((fun (x: CardInfo) -> x.typeLine), (fun (x: CardInfo) (value: string) -> { x with typeLine = value }))

    let cmc =
        Lens((fun (x: CardInfo) -> x.cmc), (fun (x: CardInfo) (value: uint) -> { x with cmc = value }))
namespace rec MagicCollectionHelper.Core

module Card =
    open MagicCollectionHelper.Core.CardTypes
    let foil (x: Card) = x.foil
    let language (x: Card) = x.language
    let number (x: Card) = x.number
    let set (x: Card) = x.set
    let create (foil: bool) (language: Language) (number: CollectorNumber) (set: MagicSet) : Card =
        { foil = foil
          language = language
          number = number
          set = set }

    let map
        (mapfoil: bool -> bool)
        (maplanguage: Language -> Language)
        (mapnumber: CollectorNumber -> CollectorNumber)
        (mapset: MagicSet -> MagicSet)
        (record': Card)
        =
        { record' with
              foil = mapfoil record'.foil
              language = maplanguage record'.language
              number = mapnumber record'.number
              set = mapset record'.set }
namespace rec MagicCollectionHelper.Core

module Entry =
    open MagicCollectionHelper.Core.CardTypes
    let amount (x: Entry) = x.amount
    let card (x: Entry) = x.card
    let create (amount: uint) (card: Card) : Entry = { amount = amount; card = card }
    let map (mapamount: uint -> uint) (mapcard: Card -> Card) (record': Entry) =
        { record' with
              amount = mapamount record'.amount
              card = mapcard record'.card }
namespace rec MagicCollectionHelper.Core

module CardInfo =
    open MagicCollectionHelper.Core.CardTypes
    let name (x: CardInfo) = x.name
    let set (x: CardInfo) = x.set
    let collectorNumber (x: CardInfo) = x.collectorNumber
    let colors (x: CardInfo) = x.colors
    let colorIdentity (x: CardInfo) = x.colorIdentity
    let oracleId (x: CardInfo) = x.oracleId
    let rarity (x: CardInfo) = x.rarity
    let typeLine (x: CardInfo) = x.typeLine
    let cmc (x: CardInfo) = x.cmc
    let create
        (name: string)
        (set: MagicSet)
        (collectorNumber: CollectorNumber)
        (colors: Set<Color>)
        (colorIdentity: ColorIdentity)
        (oracleId: string)
        (rarity: Rarity)
        (typeLine: string)
        (cmc: uint)
        : CardInfo =
        { name = name
          set = set
          collectorNumber = collectorNumber
          colors = colors
          colorIdentity = colorIdentity
          oracleId = oracleId
          rarity = rarity
          typeLine = typeLine
          cmc = cmc }

    let map
        (mapname: string -> string)
        (mapset: MagicSet -> MagicSet)
        (mapcollectorNumber: CollectorNumber -> CollectorNumber)
        (mapcolors: Set<Color> -> Set<Color>)
        (mapcolorIdentity: ColorIdentity -> ColorIdentity)
        (maporacleId: string -> string)
        (maprarity: Rarity -> Rarity)
        (maptypeLine: string -> string)
        (mapcmc: uint -> uint)
        (record': CardInfo)
        =
        { record' with
              name = mapname record'.name
              set = mapset record'.set
              collectorNumber = mapcollectorNumber record'.collectorNumber
              colors = mapcolors record'.colors
              colorIdentity = mapcolorIdentity record'.colorIdentity
              oracleId = maporacleId record'.oracleId
              rarity = maprarity record'.rarity
              typeLine = maptypeLine record'.typeLine
              cmc = mapcmc record'.cmc }
namespace rec MagicCollectionHelper.Core

module Rarity =
    open MagicCollectionHelper.Core.CardTypes
    let toString (x: Rarity) =
        match x with
        | Common -> "Common"
        | Uncommon -> "Uncommon"
        | Rare -> "Rare"
        | Mythic -> "Mythic"
        | Special -> "Special"
        | Bonus -> "Bonus"

    let fromString (x: string) =
        match x with
        | "Common" -> Some Common
        | "Uncommon" -> Some Uncommon
        | "Rare" -> Some Rare
        | "Mythic" -> Some Mythic
        | "Special" -> Some Special
        | "Bonus" -> Some Bonus
        | _ -> None

    let toTag (x: Rarity) =
        match x with
        | Common -> 0
        | Uncommon -> 1
        | Rare -> 2
        | Mythic -> 3
        | Special -> 4
        | Bonus -> 5

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

    let isSpecial (x: Rarity) =
        match x with
        | Special -> true
        | _ -> false

    let isBonus (x: Rarity) =
        match x with
        | Bonus -> true
        | _ -> false
