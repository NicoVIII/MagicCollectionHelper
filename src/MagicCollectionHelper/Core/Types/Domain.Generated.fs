//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.Core.Types

module PrefsLenses =
    open MagicCollectionHelper.Core.Types
    let dozenalize =
        Lens((fun (x: Prefs) -> x.dozenalize), (fun (x: Prefs) (value: bool) -> { x with dozenalize = value }))

    let missingPercent =
        Lens((fun (x: Prefs) -> x.missingPercent), (fun (x: Prefs) (value: float) -> { x with missingPercent = value }))

    let setWithFoils =
        Lens((fun (x: Prefs) -> x.setWithFoils), (fun (x: Prefs) (value: bool) -> { x with setWithFoils = value }))
namespace rec MagicCollectionHelper.Core.Types.DUs

module Rule =
    open MagicCollectionHelper.Core.Types
    let toString (x: Rule) =
        match x with
        | InSet _ -> "InSet"
        | InLanguage _ -> "InLanguage"
        | IsFoil _ -> "IsFoil"
        | Limit _ -> "Limit"

    let fromString (x: string) =
        match x with
        | _ -> None

    let toTag (x: Rule) =
        match x with
        | InSet _ -> 0
        | InLanguage _ -> 1
        | IsFoil _ -> 2
        | Limit _ -> 3

    let isInSet (x: Rule) =
        match x with
        | InSet _ -> true
        | _ -> false

    let isInLanguage (x: Rule) =
        match x with
        | InLanguage _ -> true
        | _ -> false

    let isIsFoil (x: Rule) =
        match x with
        | IsFoil _ -> true
        | _ -> false

    let isLimit (x: Rule) =
        match x with
        | Limit _ -> true
        | _ -> false
