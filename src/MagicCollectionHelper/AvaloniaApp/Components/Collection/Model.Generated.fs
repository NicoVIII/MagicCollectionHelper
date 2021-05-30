//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated

module StateLenses =
    open MagicCollectionHelper.AvaloniaApp.Components.Collection
    let loadInProgress =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: State) -> x.loadInProgress),
            (fun (x: State) (value: bool) -> { x with loadInProgress = value })
        )
namespace rec MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated

module Msg =
    open MagicCollectionHelper.AvaloniaApp.Components.Collection
    let toString (x: Msg) =
        match x with
        | ImportCollection -> "ImportCollection"
        | WriteCollection _ -> "WriteCollection"
        | SaveCollection _ -> "SaveCollection"

    let fromString (x: string) =
        match x with
        | "ImportCollection" -> Some ImportCollection
        | _ -> None

    let toTag (x: Msg) =
        match x with
        | ImportCollection -> 0
        | WriteCollection _ -> 1
        | SaveCollection _ -> 2

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
