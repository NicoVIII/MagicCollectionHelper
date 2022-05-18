namespace MagicCollectionHelper.Core

open SimpleOptics

[<AutoOpen>]
module DomainOptics =
    [<RequireQualifiedAccess>]
    module PrefsOptic =
        let cardGroupMinSize =
            Lens((fun prefs -> prefs.cardGroupMinSize), (fun prefs value -> { prefs with cardGroupMinSize = value }))

        let cardGroupMaxSize =
            Lens((fun prefs -> prefs.cardGroupMaxSize), (fun prefs value -> { prefs with cardGroupMaxSize = value }))

        let missingPercent =
            Lens((fun prefs -> prefs.missingPercent), (fun prefs value -> { prefs with missingPercent = value }))

        let numBase =
            Lens((fun prefs -> prefs.numBase), (fun prefs value -> { prefs with numBase = value }))

        let setWithFoils =
            Lens((fun prefs -> prefs.setWithFoils), (fun prefs value -> { prefs with setWithFoils = value }))

    [<RequireQualifiedAccess>]
    module CustomLocationOptic =
        let rules =
            Lens(
                (fun (customLocation: CustomLocation) -> customLocation.rules),
                (fun customLocation value -> { customLocation with rules = value })
            )
