module MagicCollectionHelper.AvaloniaApp.Components.Collection.View

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.Components
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Collection
open MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated
open MagicCollectionHelper.AvaloniaApp.Elements

let topBar entries (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    ActionButtonBar.create [
        ActionButton.create
            { text = "Import collection"
              isEnabled = List.isEmpty entries && not (loadInProgress)
              action = (fun _ -> ImportCollection |> dispatch) }
        ActionButton.create
            { text = "Import decks"
              isEnabled = false
              action = (fun _ -> ()) }
    ]

let renderText entries (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    TextBlock.create [
        match loadInProgress, List.isEmpty entries with
        | true, _ -> TextBlock.text "Loading..."
        | false, true -> TextBlock.text "Your collection is empty. Import it first."
        | false, false ->
            let cardAmount =
                List.sumBy (fun (entry: DeckStatsCardEntry) -> entry.amount) entries

            TextBlock.text $"You have %i{cardAmount} cards in your collection."
    ]
    :> IView

let content entries (state: State) (dispatch: Dispatch) : IView =
    Border.create [
        Border.padding 10.
        Border.child (renderText entries state dispatch)
    ]
    :> IView

let render entries (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            topBar entries state dispatch
            content entries state dispatch
        ]
    ]
    :> IView
