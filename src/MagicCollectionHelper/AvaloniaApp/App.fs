namespace MagicCollectionHelper.AvaloniaApp

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

[<RequireQualifiedAccess>]
module App =
    type State = {
        cardInfo: IReadable<CardInfoMap>
        dsEntries: IWritable<DeckStatsCardEntry list>
        entries: IReadable<AgedEntry list>
        prefs: IWritable<Prefs>
        setData: IReadable<SetDataMap>
    }

    let renderCollectionView state : IView =
        Components.Collection.View.view
            {
                cardInfo = state.cardInfo
                dsEntries = state.dsEntries
                entries = state.entries
                prefs = state.prefs
            }

    let renderInventoryView state : IView =
        Components.Inventory.View.view
            {
                cardInfo = state.cardInfo
                entries = state.entries
                prefs = state.prefs
                setData = state.setData
            }

    let createTab (header: string) content =
        let content =
            Border.create [
                Border.borderThickness (0., 1., 0., 0.)
                Border.borderBrush Config.lineColor
                Border.child (content: IView)
            ]

        TabItem.create [ TabItem.fontSize 22.; TabItem.header header; TabItem.content content ]

    let createTabs state : IView list = [
        createTab "Collection" (renderCollectionView state)
        createTab "Inventory" (renderInventoryView state)
        createTab "Preferences" (Tab.Preference.render state.prefs)
    ]

    let render (state: State) : IView =
        TabControl.create [
            TabControl.tabStripPlacement Dock.Top // Change this property to tell the app where to show the tab bar
            TabControl.viewItems (createTabs state)
        ]
        :> IView

    type Input = {
        cardInfo: IReadable<CardInfoMap>
        dsEntries: IWritable<DeckStatsCardEntry list>
        entries: IReadable<AgedEntry list>
        prefs: IWritable<Prefs>
        setData: IReadable<SetDataMap>
    }

    let create input =
        Component.create (
            "app",
            fun (ctx) ->
                let state: State = {
                    cardInfo = ctx.usePassedRead input.cardInfo
                    dsEntries = ctx.usePassed input.dsEntries
                    entries = ctx.usePassedRead input.entries
                    prefs = ctx.usePassed input.prefs
                    setData = ctx.usePassedRead input.setData
                }

                render state
        )
