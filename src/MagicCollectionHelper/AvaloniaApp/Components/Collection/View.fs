module MagicCollectionHelper.AvaloniaApp.Components.Collection.View

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp.Components.Collection
open MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated
open MagicCollectionHelper.AvaloniaApp.Elements
open Avalonia.Media

let topBar (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    ActionButtonBar.create [
        ActionButton.create
            { text = "Import collection"
              isEnabled = not loadInProgress
              action = (fun _ -> ImportCollection |> dispatch)
              subPatch = Never }
    ]

let renderText prefs dsEntries agedEntriesWithInfo (state: State) (dispatch: Dispatch) : IView =
    let loadInProgress = getl StateLenses.loadInProgress state

    let inventoryableAmount = List.sumBy (getl AgedEntryWithInfoLenses.amount) agedEntriesWithInfo

    TextBlock.create [
        TextBlock.textWrapping TextWrapping.Wrap
        TextBlock.text (
            match loadInProgress, List.isEmpty agedEntriesWithInfo with
            | true, _ -> "Loading..."
            | false, true -> "Your collection is empty. Import it first."
            | false, false ->
                let cardAmount =
                    List.sumBy (fun (entry: DeckStatsCardEntry) -> entry.amount) dsEntries

                let percent =
                    (double inventoryableAmount) / (double cardAmount)
                    |> Numbers.percent prefs.numBase

                let inline pN p = Numbers.print prefs.numBase p

                $"You have %s{pN 0 cardAmount} cards in your collection.\n\n"
                + $"From those you can use {pN 0 inventoryableAmount} ({pN 1 percent}%%) for inventory."
                + " If you need more, please extend the info in your collection. Especially important are set, collector number and language."
        )
    ]
    :> IView

let headerItem column label =
    Border.create [
        Border.row 0
        Border.column column
        Border.borderThickness (0., 0., 0., 2.)
        Border.child (
            Button.create [
                Button.content (label: string)
            ]
        )
    ]
    :> IView

let entryRow columns i entry =
    [ for (j, (_, get)) in List.indexed columns do
          Border.create [
              Border.row i
              Border.column (2 * j)
              Border.padding 5.
              Border.child (
                  TextBlock.create [
                      TextBlock.text (get entry)
                  ]
              )
          ]
          :> IView ]

let tableView entries state =
    let columns =
        [ "Name", getl AgedEntryWithInfoLenses.name
          "Set",
          (getl AgedEntryWithInfoLenses.set
           >> MagicSet.unwrap) ]

    // Paging
    let entries =
        entries
        |> List.skip state.offset
        |> List.take state.limit

    Grid.create [
        Grid.columnDefinitions (
            List.replicate (List.length columns) "1*"
            |> String.concat ",Auto,"
        )
        Grid.rowDefinitions (
            List.replicate (List.length entries + 1) "Auto"
            |> String.concat ","
        )
        Grid.children [
            // Header
            for (i, (name, _)) in List.indexed columns do
                headerItem (2 * i) name

                if i < List.length columns - 1 then
                    GridSplitter.create [
                        GridSplitter.column (2 * i + 1)
                    ]

            // Rows
            for (i, entry) in List.indexed entries do
                yield! entryRow columns (i + 1) entry
        ]
    ]
    :> IView

let content prefs dsEntries agedEntriesWithInfo (state: State) (dispatch: Dispatch) : IView =
    StackPanel.create [
        StackPanel.children [
            Border.create [
                Border.padding 10.
                Border.child (renderText prefs dsEntries agedEntriesWithInfo state dispatch)
            ]
            ScrollViewer.create [
                ScrollViewer.content (tableView agedEntriesWithInfo state)
            ]
        ]
    ]
    :> IView

let render prefs dsEntries agedEntriesWithInfo (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            topBar state dispatch
            content prefs dsEntries agedEntriesWithInfo state dispatch
        ]
    ]
    :> IView
