module MagicCollectionHelper.AvaloniaApp.Main.Ready.View

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Main.Ready.ViewComponents

let renderCollectionView state dispatch =
    let prefs = state ^. StateOptic.prefs
    let cardInfo = state ^. StateOptic.cardInfo
    let dsEntries = state ^. StateOptic.dsEntries
    let entries = state ^. StateOptic.entries
    let dispatch = CollectionMsg >> dispatch

    let agedEntriesWithInfo =
        entries |> List.map (AgedEntryWithInfo.fromEntry cardInfo) |> List.choose id

    Components.Collection.View.render prefs dsEntries agedEntriesWithInfo state.collection dispatch

let renderInventoryView state dispatch =
    let prefs = state ^. StateOptic.prefs
    let entries = state ^. StateOptic.entries
    let cardInfo = state ^. StateOptic.cardInfo
    Components.Inventory.View.render prefs cardInfo entries state.inventory (InventoryMsg >> dispatch)

let createTab (header: string) (content: IView) =
    let content =
        Border.create [
            Border.borderThickness (0., 1., 0., 0.)
            Border.borderBrush Config.lineColor
            Border.child content
        ]

    TabItem.create [ TabItem.fontSize 22.; TabItem.header header; TabItem.content content ]

let createTabs state dispatch : IView list = [
    createTab "Collection" (renderCollectionView state dispatch)
    createTab "Inventory" (renderInventoryView state dispatch)
    createTab "Preferences" (PreferenceView.render state dispatch)
]

let render (state: State) (dispatch: Dispatch) : IView =
    TabControl.create [
        TabControl.tabStripPlacement Dock.Top // Change this property to tell the app where to show the tab bar
        TabControl.viewItems (createTabs state dispatch)
    ]
    :> IView

open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish.ElmishHook

type Input = {
    cardInfo: IReadable<CardInfoMap>
    dsEntries: IReadable<DeckStatsCardEntry list>
    entries: IReadable<Entry list>
    setData: IReadable<SetDataMap>
}

let view input =
    Component.create (
        "ReadyView",
        fun (ctx) ->
            let cardInfo = ctx.usePassedRead input.cardInfo
            let dsEntries = ctx.usePassedRead input.dsEntries
            let entries = ctx.usePassedRead input.entries
            let setData = ctx.usePassedRead input.setData

            let state, dispatch =
                ctx.useElmish (
                    Model.init cardInfo.Current dsEntries.Current entries.Current setData.Current,
                    Update.perform
                )

            render state dispatch
    )
