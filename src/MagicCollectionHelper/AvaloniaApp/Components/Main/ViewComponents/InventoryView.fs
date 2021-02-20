namespace MagicCollectionHelper.AvaloniaApp.Components.Main.ViewComponents

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.Components
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Markup.Xaml.Templates
open Avalonia.Media

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Main
open MagicCollectionHelper.AvaloniaApp.Elements

module InventoryView =
    let topBar (state: State) (dispatch: Dispatch): IView =
        ActionButtonBar.create [
            ActionButton.create {
                text = "Take inventory"
                isEnabled = (not state.cards.IsEmpty)
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
        ScrollViewer.create [
            ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
            ScrollViewer.content(
                TreeView.create [
                    TreeView.dataItems(
                        state.inventory
                        |> Map.toList
                        |> List.sortBy (fun (l, _) ->
                            // We sort like our locations are sorted
                            List.tryFindIndex (fun l' -> Custom l' = l) state.locations
                            |> Option.defaultValue 999))
                    TreeView.itemTemplate (DataTemplateView<InventoryLocation * CardEntry list>.create(locationItem))
                ]
            )
        ] :> IView

    let render (state: State) (dispatch: Dispatch): IView =
        DockPanel.create [
            DockPanel.children [
                topBar state dispatch
                content state dispatch
            ]
        ]:> IView
