module MagicCollectionHelper.AvaloniaApp.Components.Inventory.View

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.Components
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.ViewComponents
open MagicCollectionHelper.AvaloniaApp.Elements

let topBar (entries: CardEntry list) (state: State) (dispatch: Dispatch): IView =
    ActionButtonBar.create [
        ActionButton.create {
            text = "Take inventory"
            isEnabled = (not (entries.IsEmpty || state.loadInProgress))
            action = (fun _ -> TakeInventory |> dispatch)
        }
    ]

let cardItem (infoMap: CardInfoMap) (entry: CardEntry) =
    let amount =
        match entry.amount with
        | amount when amount < 10u ->
            $" {amount}"
        | amount ->
            $"{amount}"
    let cardInfo =
        infoMap.Item (entry.set, entry.number)

    TextBlock.create [
        TextBlock.text $"{amount} {cardInfo.name}"
    ]

type TestMe =
    {
        l: InventoryLocation
        e: CardEntry seq
    }

let locationItem t =
    let (location, entryList) = (t.l, t.e)

    let sum = Seq.sumBy (fun entry -> entry.amount) entryList

    (*TreeViewItem.create [
        match location with
        | Custom location ->
            TreeViewItem.header $"{location.name} ({sum})"
        | Fallback ->
            TreeViewItem.header $"Leftover ({sum})"
        TreeViewItem.dataItems (List.sortBy (fun (entry: CardEntry) -> entry.name) entryList)
        TreeViewItem.itemTemplate (DataTemplateView<CardEntry>.create(cardItem))
    ]*)
    TextBlock.create [
        match location with
        | Custom location ->
            TextBlock.text $"{location.name} ({sum})"
        | Fallback ->
            TextBlock.text $"Leftover ({sum})"
    ]

let content (state: State) (dispatch: Dispatch): IView =
    match state.editLocations, state.loadInProgress with
    | true, _ ->
        LocationEdit.render state dispatch
    | false, true ->
        Border.create [
            Border.padding 10.
            Border.child(
                TextBlock.create [
                    TextBlock.text "Loading..."
                ])
        ] :> IView
    | false, false ->
        TreeView.create [
            TreeView.dataItems(
                state.inventory
                |> Map.toList
                |> List.sortBy (fun (l, _) ->
                    // We sort like our locations are sorted
                    List.tryFindIndex (fun l' -> Custom l' = l) state.locations
                    |> Option.defaultValue 999)
                |> List.map (fun (l, e) -> { l = l; e = e }))
            TreeView.itemTemplate (DataTemplateView<TestMe>.create((fun t -> t.e), locationItem))
        ] :> IView

let render (entries: CardEntry list) (state: State) (dispatch: Dispatch): IView =
    DockPanel.create [
        DockPanel.children [
            topBar entries state dispatch
            Button.create [
                Button.dock Dock.Top
                Button.content (if state.editLocations then "Close" else "Edit")
                Button.onClick ((fun _ ->
                    if state.editLocations then
                        CloseLocationEdit
                    else
                        OpenLocationEdit
                    |> dispatch), SubPatchOptions.Always)
            ]
            ScrollViewer.create [
                ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                ScrollViewer.content (content state dispatch)
            ]
        ]
    ]:> IView
