//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.AvaloniaApp.Components.Inventory.Generated

module StateLenses =
    open MagicCollectionHelper.AvaloniaApp.Components.Inventory

    let filteredInventory =
        SimpleOptics.Lens(
            (fun (x: State) -> x.filteredInventory),
            (fun (x: State) (value: LocationWithHungTree list) -> { x with filteredInventory = value })
        )

    let inventory =
        SimpleOptics.Lens(
            (fun (x: State) -> x.inventory),
            (fun (x: State) (value: MagicCollectionHelper.Core.DomainTypes.LocationWithCards) ->
                { x with inventory = value })
        )

    let locations =
        SimpleOptics.Lens(
            (fun (x: State) -> x.locations),
            (fun (x: State) (value: MagicCollectionHelper.Core.DomainTypes.CustomLocation list) ->
                { x with locations = value })
        )

    let search =
        SimpleOptics.Lens(
            (fun (x: State) -> x.search),
            (fun (x: State) (value: MagicCollectionHelper.AvaloniaApp.DomainTypes.Search) -> { x with search = value })
        )

    let viewMode =
        SimpleOptics.Lens(
            (fun (x: State) -> x.viewMode),
            (fun (x: State) (value: ViewMode) -> { x with viewMode = value })
        )


namespace rec MagicCollectionHelper.AvaloniaApp.Components.Inventory.Generated

module Msg =
    open MagicCollectionHelper.AvaloniaApp.Components.Inventory

    let toString (x: Msg) =
        match x with
        | AsyncError _ -> "AsyncError"
        | ChangeState _ -> "ChangeState"
        | TakeInventory -> "TakeInventory"
        | SaveInventory _ -> "SaveInventory"
        | OpenLocationEdit -> "OpenLocationEdit"
        | CloseLocationEdit -> "CloseLocationEdit"
        | FilterInventory _ -> "FilterInventory"
        | ChangeLocation _ -> "ChangeLocation"
        | UpdateLocationRules _ -> "UpdateLocationRules"

    let fromString (x: string) =
        match x with
        | "TakeInventory" -> Some TakeInventory
        | "OpenLocationEdit" -> Some OpenLocationEdit
        | "CloseLocationEdit" -> Some CloseLocationEdit
        | _ -> None

    let toTag (x: Msg) =
        match x with
        | AsyncError _ -> 0
        | ChangeState _ -> 1
        | TakeInventory -> 2
        | SaveInventory _ -> 3
        | OpenLocationEdit -> 4
        | CloseLocationEdit -> 5
        | FilterInventory _ -> 6
        | ChangeLocation _ -> 7
        | UpdateLocationRules _ -> 8

    let isAsyncError (x: Msg) =
        match x with
        | AsyncError _ -> true
        | _ -> false

    let isChangeState (x: Msg) =
        match x with
        | ChangeState _ -> true
        | _ -> false

    let isTakeInventory (x: Msg) =
        match x with
        | TakeInventory -> true
        | _ -> false

    let isSaveInventory (x: Msg) =
        match x with
        | SaveInventory _ -> true
        | _ -> false

    let isOpenLocationEdit (x: Msg) =
        match x with
        | OpenLocationEdit -> true
        | _ -> false

    let isCloseLocationEdit (x: Msg) =
        match x with
        | CloseLocationEdit -> true
        | _ -> false

    let isFilterInventory (x: Msg) =
        match x with
        | FilterInventory _ -> true
        | _ -> false

    let isChangeLocation (x: Msg) =
        match x with
        | ChangeLocation _ -> true
        | _ -> false

    let isUpdateLocationRules (x: Msg) =
        match x with
        | UpdateLocationRules _ -> true
        | _ -> false

