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