namespace MagicCollectionHelper.AvaloniaApp.Components.Collection

open SimpleOptics

[<AutoOpen>]
module ModelOptics =
    [<RequireQualifiedAccess>]
    module StateOptic =
        let loadInProgress =
            Lens((fun state -> state.loadInProgress), (fun state value -> { state with loadInProgress = value }))

        let pageOffset =
            Lens((fun state -> state.pageOffset), (fun state value -> { state with pageOffset = value }))

        let pageSize =
            Lens((fun state -> state.pageSize), (fun state value -> { state with pageSize = value }))
