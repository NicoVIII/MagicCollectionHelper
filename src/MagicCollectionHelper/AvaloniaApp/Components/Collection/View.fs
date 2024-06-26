namespace MagicCollectionHelper.AvaloniaApp.Components.Collection

open SimpleOptics
open System

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Collection
open MagicCollectionHelper.AvaloniaApp.ViewComponent

module View =
    let buttonBar (state: State) (dispatch: Dispatch) : IView =
        let loadInProgress = Optic.get StateOptic.loadInProgress state

        ActionButtonBar.create [
            ActionButton.create {
                text = "Import collection"
                isEnabled = not loadInProgress
                action = (fun _ -> ImportCollection |> dispatch)
                subPatch = Never
            }
        ]

    let renderText prefs dsEntries agedEntriesWithInfo (state: State) (dispatch: Dispatch) : IView =
        let loadInProgress = Optic.get StateOptic.loadInProgress state

        let inventoryableAmount =
            List.sumBy (Optic.get AgedEntryWithInfoOptic.amount) agedEntriesWithInfo

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
            Border.child (Button.create [ Button.content (label: string) ])
        ]
        :> IView

    let entryRow columns i entry = [
        for (j, (_, get)) in List.indexed columns do
            Border.create [
                Border.row i
                Border.column (2 * j)
                Border.padding 5.
                Border.child (TextBlock.create [ TextBlock.text (get entry) ])
            ]
            :> IView
    ]

    let pagingBar entries (state: State) dispatch =
        let pageAmount =
            (List.length entries |> double) / (state.pageSize |> double)
            |> Math.Ceiling
            |> int

        Border.create [
            Border.borderBrush Config.lineColor
            Border.borderThickness (0., 1., 0., 0.)
            Border.dock Dock.Bottom
            Border.child (
                DockPanel.create [
                    DockPanel.dock Dock.Bottom
                    DockPanel.children [
                        TextBlock.create [
                            TextBlock.dock Dock.Right
                            TextBlock.margin (0., 0., 5., 0.)
                            TextBlock.text $"{state.pageOffset + 1} / {pageAmount}"
                            TextBlock.verticalAlignment VerticalAlignment.Center
                        ]
                        StackPanel.create [
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.horizontalAlignment HorizontalAlignment.Center
                            StackPanel.children [
                                Button.create [
                                    Button.content "<"
                                    Button.isEnabled (state.pageOffset > 0)
                                    Button.onClick (fun _ -> (+) -1 |> ChangePage |> dispatch)
                                ]
                                ComboBox.create [
                                    ComboBox.dataItems [ 10; 25; 50; 100; 250 ]
                                    ComboBox.selectedItem state.pageSize
                                    ComboBox.onSelectedItemChanged (unbox<int> >> SetPageSize >> dispatch)
                                ]
                                Button.create [
                                    Button.content ">"
                                    Button.isEnabled ((state.pageOffset + 1) * state.pageSize < List.length entries)
                                    Button.onClick (fun _ -> (+) 1 |> ChangePage |> dispatch)
                                ]
                            ]
                        ]
                    ]
                ]
            )
        ]
        :> IView

    module Optic = AgedEntryWithInfoOptic

    let tableView entries state =
        let columns = [
            "Name", Optic.get Optic.name
            "Set", (Optic.get Optic.set) >> MagicSet.unwrap
            "Nr.", (Optic.get Optic.number) >> CollectorNumber.unwrap
            "Language", (Optic.get Optic.language) >> Language.unwrap
        ]

        // Paging
        let entries =
            match List.length entries with
            | length when length >= (state.pageOffset + 1) * state.pageSize ->
                entries
                |> List.skip (state.pageOffset * state.pageSize)
                |> List.take state.pageSize
            | length when length > state.pageOffset * state.pageSize ->
                entries |> List.skip (state.pageOffset * state.pageSize)
            | _ -> []

        Grid.create [
            Grid.columnDefinitions (List.replicate (List.length columns) "1*" |> String.concat ",Auto,")
            Grid.rowDefinitions (List.replicate (List.length entries + 1) "Auto" |> String.concat ",")
            Grid.children [
                // Header
                for (i, (name, _)) in List.indexed columns do
                    headerItem (2 * i) name

                    if i < List.length columns - 1 then
                        GridSplitter.create [ GridSplitter.column (2 * i + 1) ]

                // Rows
                for (i, entry) in List.indexed entries do
                    yield! entryRow columns (i + 1) entry
            ]
        ]
        :> IView

    let collectionView entries state dispatch =
        Border.create [
            Border.borderBrush Config.lineColor
            Border.borderThickness (0., 1., 0., 0.)
            Border.child (
                DockPanel.create [
                    DockPanel.children [
                        pagingBar entries state dispatch
                        ScrollViewer.create [
                            ScrollViewer.verticalScrollBarVisibility ScrollBarVisibility.Visible
                            ScrollViewer.content (tableView entries state)
                        ]
                    ]
                ]
            )
        ]
        :> IView

    let content prefs dsEntries agedEntriesWithInfo (state: State) (dispatch: Dispatch) : IView =
        DockPanel.create [
            DockPanel.children [
                Border.create [
                    Border.dock Dock.Top
                    Border.padding (10., 10., 10., 20.)
                    Border.child (renderText prefs dsEntries agedEntriesWithInfo state dispatch)
                ]
                collectionView agedEntriesWithInfo state dispatch
            ]
        ]
        :> IView

    let render prefs dsEntries agedEntriesWithInfo (state: State) (dispatch: Dispatch) : IView =
        DockPanel.create [
            DockPanel.children [
                buttonBar state dispatch
                content prefs dsEntries agedEntriesWithInfo state dispatch
            ]
        ]
        :> IView

    open Avalonia.FuncUI
    open Avalonia.FuncUI.Elmish.ElmishHook

    type Input = {
        cardInfo: IReadable<CardInfoMap>
        dsEntries: IWritable<DeckStatsCardEntry list>
        entries: IReadable<AgedEntry list>
        prefs: IReadable<Prefs>
    }

    let view input =
        Component.create (
            "collection-tab",
            fun ctx ->
                let cardInfo = ctx.usePassedRead input.cardInfo
                let dsEntries = ctx.usePassed input.dsEntries
                let entries = ctx.usePassedRead input.entries
                let prefs = ctx.usePassedRead input.prefs

                let agedInfo =
                    entries.Current
                    |> List.map (AgedEntryWithInfo.fromEntry cardInfo.Current)
                    |> List.choose id

                let state, dispatch = ctx.useElmish (Model.init, Update.perform dsEntries)
                render prefs.Current dsEntries.Current agedInfo state dispatch
        )
