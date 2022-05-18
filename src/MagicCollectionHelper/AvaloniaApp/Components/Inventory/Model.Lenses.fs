namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory

open SimpleOptics

[<AutoOpen>]
module ModelLenses =
    [<RequireQualifiedAccess>]
    module StateLenses =
        let filteredInventory =
            Lens((fun state -> state.filteredInventory), (fun state value -> { state with filteredInventory = value }))

        let inventory =
            Lens((fun state -> state.inventory), (fun state value -> { state with inventory = value }))

        let locations =
            Lens((fun state -> state.locations), (fun state value -> { state with locations = value }))

        let search =
            Lens((fun state -> state.search), (fun state value -> { state with search = value }))

        let viewMode =
            Lens((fun state -> state.viewMode), (fun state value -> { state with viewMode = value }))
