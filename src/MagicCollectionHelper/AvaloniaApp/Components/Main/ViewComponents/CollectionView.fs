namespace MagicCollectionHelper.AvaloniaApp.Components.Main.ViewComponents

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Main
open MagicCollectionHelper.AvaloniaApp.Elements

module CollectionView =
    let topBar (state: State) (dispatch: Dispatch): IView =
        ActionButtonBar.create [
            ActionButton.create {
                text = "Import collection"
                isEnabled = (state.cards.IsEmpty)
                action = (fun _ -> ImportCollection |> dispatch)
            }
            ActionButton.create {
                text = "Import decks"
                isEnabled = false
                action = (fun _ -> ())
            }
        ]

    let content (state: State) (dispatch: Dispatch): IView =
        Border.create [
            Border.padding 10.
            Border.child(
                TextBlock.create [
                    TextBlock.text "TBD - This should become a collection explorer"
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
