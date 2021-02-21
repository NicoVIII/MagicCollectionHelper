module MagicCollectionHelper.AvaloniaApp.Components.Inventory.View

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.Components
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components.Inventory
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
    TextBlock.create [
        TextBlock.text entry.name
    ]

let locationItem (location, (entryList: CardEntry list)) =
    TreeViewItem.create [
        match location with
        | Custom location ->
            TreeViewItem.header location.name
        | Fallback ->
            TreeViewItem.header "Leftover"
        TreeViewItem.dataItems entryList
        TreeViewItem.itemTemplate (DataTemplateView<CardEntry>.create(cardItem))
    ]

let content (state: State) (dispatch: Dispatch): IView =
    if state.loadInProgress then
        Border.create [
            Border.padding 10.
            Border.child(
                TextBlock.create [
                    TextBlock.text "Loading..."
                ])
        ] :> IView
    else
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
            ScrollViewer.create [
                ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                ScrollViewer.content (content state dispatch)
            ]
        ]
    ]:> IView
