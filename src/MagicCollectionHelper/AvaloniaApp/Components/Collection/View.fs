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
open Avalonia.Media

let topBar entries infoMap (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    ActionButtonBar.create [
        ActionButton.create
            { text = "Import collection"
              isEnabled =
                  List.isEmpty entries
                  && not (loadInProgress)
                  && not (Map.isEmpty infoMap)
              action = (fun _ -> ImportCollection |> dispatch) }
        ActionButton.create
            { text = "Import decks"
              isEnabled = false
              action = (fun _ -> ()) }
    ]

let renderText dsEntries entries infoMap (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    let inventoryableAmount =
        List.sumBy
            (function
            | (entry: CardEntry) when Map.containsKey (entry.card.set, entry.card.number) infoMap -> entry.amount
            | _ -> 0u)
            entries

    let cardAmount =
        List.sumBy (fun (entry: DeckStatsCardEntry) -> entry.amount) dsEntries

    TextBlock.create [
        TextBlock.textWrapping TextWrapping.Wrap
        TextBlock.text (
            match loadInProgress, List.isEmpty entries with
            | true, _ -> "Loading..."
            | false, true -> "Your collection is empty. Import it first."
            | false, false ->
                let percent =
                    (double inventoryableAmount) / (double cardAmount)
                    * 100.

                $"You have %i{cardAmount} cards in your collection.\n\n"
                + $"From those you can use {inventoryableAmount} (%.1f{percent}%%) for inventory."
                + " If you need more, please extend the info in your collection. Especially important are set and collector number."
        )
    ]
    :> IView

let content dsEntries entries infoMap (state: State) (dispatch: Dispatch) : IView =
    Border.create [
        Border.padding 10.
        Border.child (renderText dsEntries entries infoMap state dispatch)
    ]
    :> IView

let render dsEntries entries infoMap (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            topBar entries infoMap state dispatch
            content dsEntries entries infoMap state dispatch
        ]
    ]
    :> IView