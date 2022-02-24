//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated

module StateLenses =
    open MagicCollectionHelper.AvaloniaApp.Components.Collection

    let loadInProgress =
        MagicCollectionHelper.Core.Lens(
            (fun (x: State) -> x.loadInProgress),
            (fun (x: State) (value: bool) -> { x with loadInProgress = value })
        )

    let pageSize =
        MagicCollectionHelper.Core.Lens(
            (fun (x: State) -> x.pageSize),
            (fun (x: State) (value: int) -> { x with pageSize = value })
        )

    let pageOffset =
        MagicCollectionHelper.Core.Lens(
            (fun (x: State) -> x.pageOffset),
            (fun (x: State) (value: int) -> { x with pageOffset = value })
        )


namespace rec MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated

module Msg =
    open MagicCollectionHelper.AvaloniaApp.Components.Collection

    let toString (x: Msg) =
        match x with
        | ImportCollection -> "ImportCollection"
        | WriteCollection _ -> "WriteCollection"
        | SaveCollection _ -> "SaveCollection"
        | ChangePage _ -> "ChangePage"
        | SetPageSize _ -> "SetPageSize"

    let fromString (x: string) =
        match x with
        | "ImportCollection" -> Some ImportCollection
        | _ -> None

    let toTag (x: Msg) =
        match x with
        | ImportCollection -> 0
        | WriteCollection _ -> 1
        | SaveCollection _ -> 2
        | ChangePage _ -> 3
        | SetPageSize _ -> 4

    let isImportCollection (x: Msg) =
        match x with
        | ImportCollection -> true
        | _ -> false

    let isWriteCollection (x: Msg) =
        match x with
        | WriteCollection _ -> true
        | _ -> false

    let isSaveCollection (x: Msg) =
        match x with
        | SaveCollection _ -> true
        | _ -> false

    let isChangePage (x: Msg) =
        match x with
        | ChangePage _ -> true
        | _ -> false

    let isSetPageSize (x: Msg) =
        match x with
        | SetPageSize _ -> true
        | _ -> false

