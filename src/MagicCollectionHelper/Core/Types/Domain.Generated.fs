//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.Core

module PrefsLenses =
    open MagicCollectionHelper.Core.DomainTypes
    let cardGroupMinSize =
        Lens(
            (fun (x: Prefs) -> x.cardGroupMinSize),
            (fun (x: Prefs) (value: uint) -> { x with cardGroupMinSize = value })
        )

    let cardGroupMaxSize =
        Lens(
            (fun (x: Prefs) -> x.cardGroupMaxSize),
            (fun (x: Prefs) (value: uint) -> { x with cardGroupMaxSize = value })
        )

    let numBase =
        Lens((fun (x: Prefs) -> x.numBase), (fun (x: Prefs) (value: NumBase) -> { x with numBase = value }))

    let missingPercent =
        Lens((fun (x: Prefs) -> x.missingPercent), (fun (x: Prefs) (value: float) -> { x with missingPercent = value }))

    let setWithFoils =
        Lens((fun (x: Prefs) -> x.setWithFoils), (fun (x: Prefs) (value: bool) -> { x with setWithFoils = value }))
namespace rec MagicCollectionHelper.Core

module CustomLocationLenses =
    open MagicCollectionHelper.Core.DomainTypes
    let name =
        Lens(
            (fun (x: CustomLocation) -> x.name),
            (fun (x: CustomLocation) (value: CustomLocationName) -> { x with name = value })
        )

    let rules =
        Lens((fun (x: CustomLocation) -> x.rules), (fun (x: CustomLocation) (value: Rules) -> { x with rules = value }))

    let sortBy =
        Lens(
            (fun (x: CustomLocation) -> x.sortBy),
            (fun (x: CustomLocation) (value: SortRules) -> { x with sortBy = value })
        )
namespace rec MagicCollectionHelper.Core

module Prefs =
    open MagicCollectionHelper.Core.DomainTypes
    let cardGroupMinSize (x: Prefs) = x.cardGroupMinSize
    let cardGroupMaxSize (x: Prefs) = x.cardGroupMaxSize
    let numBase (x: Prefs) = x.numBase
    let missingPercent (x: Prefs) = x.missingPercent
    let setWithFoils (x: Prefs) = x.setWithFoils
    let create
        (cardGroupMinSize: uint)
        (cardGroupMaxSize: uint)
        (numBase: NumBase)
        (missingPercent: float)
        (setWithFoils: bool)
        : Prefs =
        { cardGroupMinSize = cardGroupMinSize
          cardGroupMaxSize = cardGroupMaxSize
          numBase = numBase
          missingPercent = missingPercent
          setWithFoils = setWithFoils }

    let map
        (mapcardGroupMinSize: uint -> uint)
        (mapcardGroupMaxSize: uint -> uint)
        (mapnumBase: NumBase -> NumBase)
        (mapmissingPercent: float -> float)
        (mapsetWithFoils: bool -> bool)
        (record': Prefs)
        =
        { record' with
              cardGroupMinSize = mapcardGroupMinSize record'.cardGroupMinSize
              cardGroupMaxSize = mapcardGroupMaxSize record'.cardGroupMaxSize
              numBase = mapnumBase record'.numBase
              missingPercent = mapmissingPercent record'.missingPercent
              setWithFoils = mapsetWithFoils record'.setWithFoils }
namespace rec MagicCollectionHelper.Core

module NumBase =
    open MagicCollectionHelper.Core.DomainTypes
    let toString (x: NumBase) =
        match x with
        | Decimal -> "Decimal"
        | Dozenal -> "Dozenal"
        | Seximal -> "Seximal"

    let fromString (x: string) =
        match x with
        | "Decimal" -> Some Decimal
        | "Dozenal" -> Some Dozenal
        | "Seximal" -> Some Seximal
        | _ -> None

    let toTag (x: NumBase) =
        match x with
        | Decimal -> 0
        | Dozenal -> 1
        | Seximal -> 2

    let isDecimal (x: NumBase) =
        match x with
        | Decimal -> true
        | _ -> false

    let isDozenal (x: NumBase) =
        match x with
        | Dozenal -> true
        | _ -> false

    let isSeximal (x: NumBase) =
        match x with
        | Seximal -> true
        | _ -> false
