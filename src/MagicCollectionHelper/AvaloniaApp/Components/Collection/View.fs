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

let renderText entries infoMap (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    let inventoryableAmount =
        if not (Map.isEmpty infoMap) then
            List.sumBy
                (fun (entry: DeckStatsCardEntry) ->
                    let entry = DeckStatsCardEntry.toEntry entry

                    match entry with
                    | Some entry when Map.containsKey (entry.card.set, entry.card.number) infoMap -> entry.amount
                    | Some _
                    | None -> 0u)
                entries
            |> Some
        else
            None

    let cardAmount =
        List.sumBy (fun (entry: DeckStatsCardEntry) -> entry.amount) entries

    TextBlock.create [
        TextBlock.textWrapping TextWrapping.Wrap
        TextBlock.text (
            match loadInProgress, List.isEmpty entries, inventoryableAmount with
            | true, _, _ -> "Loading..."
            | false, true, _ -> "Your collection is empty. Import it first."
            | false, false, None -> $"You have %i{cardAmount} cards in your collection."
            | false, false, Some inventoryableAmount ->
                let percent =
                    (double inventoryableAmount) / (double cardAmount)
                    * 100.

                $"You have %i{cardAmount} cards in your collection.\n\n"
                + $"From those you can use {inventoryableAmount} (%.1f{percent}%%) for inventory."
                + " If you need more, please extend the info in your collection. Especially important are set and collector number."
        )
    ]
    :> IView

let content entries infoMap (state: State) (dispatch: Dispatch) : IView =
    Border.create [
        Border.padding 10.
        Border.child (renderText entries infoMap state dispatch)
    ]
    :> IView

let render entries infoMap (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            topBar entries state dispatch
            content entries infoMap state dispatch
        ]
    ]
    :> IView
