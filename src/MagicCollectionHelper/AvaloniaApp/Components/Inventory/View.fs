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

let cardItem (entry: CardEntry) =
    let amount =
        match entry.amount with
        | amount when amount < 10u ->
            $" {amount}"
        | amount ->
            $"{amount}"

    TextBlock.create [
        TextBlock.text $"{amount} {entry.name}"
    ]

let locationItem (location, (entryList: CardEntry list)) =
    let sum = List.sumBy (fun entry -> entry.amount) entryList

    TreeViewItem.create [
        match location with
        | Custom location ->
            TreeViewItem.header $"{location.name} ({sum})"
        | Fallback ->
            TreeViewItem.header $"Leftover ({sum})"
        TreeViewItem.dataItems (List.sortBy (fun (entry: CardEntry) -> entry.name) entryList)
        TreeViewItem.itemTemplate (DataTemplateView<CardEntry>.create(cardItem))
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
                    |> Option.defaultValue 999))
            TreeView.itemTemplate (DataTemplateView<InventoryLocation * CardEntry list>.create(locationItem))
        ] :> IView

let render (entries: CardEntry list) (state: State) (dispatch: Dispatch): IView =
    DockPanel.create [
        DockPanel.children [
            topBar entries state dispatch
            Button.create [
                Button.dock Dock.Top
                Button.content (if state.editLocations then "Close" else "Edit")
                Button.onClick (fun _ -> OpenLocationEdit |> dispatch)
            ]
            ScrollViewer.create [
                ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                ScrollViewer.content (content state dispatch)
            ]
        ]
    ]:> IView
