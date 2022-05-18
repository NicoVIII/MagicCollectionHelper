namespace MagicCollectionHelper.AvaloniaApp.Main.Loading

open SimpleOptics

[<AutoOpen>]
module ModelOptics =
    [<RequireQualifiedAccess>]
    module StateOptic =
        let cardInfo =
            Lens((fun state -> state.cardInfo), (fun state value -> { state with cardInfo = value }))

        let dsEntries =
            Lens((fun state -> state.dsEntries), (fun state value -> { state with dsEntries = value }))

        let entries =
            Lens((fun state -> state.entries), (fun state value -> { state with entries = value }))

        let setData =
            Lens((fun state -> state.setData), (fun state value -> { state with setData = value }))
