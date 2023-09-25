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

        let numBase =
            Lens((fun prefs -> prefs.numBase), (fun prefs value -> { prefs with numBase = value }))

    [<RequireQualifiedAccess>]
    module CustomLocationOptic =
        let rules =
            Lens(
                (fun (customLocation: CustomLocation) -> customLocation.rules),
                (fun customLocation value -> { customLocation with rules = value })
            )
