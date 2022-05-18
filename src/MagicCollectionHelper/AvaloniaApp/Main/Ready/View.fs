module MagicCollectionHelper.AvaloniaApp.Main.Ready.View

open Avalonia.Controls

open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Main.Ready.ViewComponents

let renderCollectionView state dispatch =
    let prefs = state ^. StateLenses.prefs
    let cardInfo = state ^. StateLenses.cardInfo
    let dsEntries = state ^. StateLenses.dsEntries
    let entries = state ^. StateLenses.entries
    let dispatch = CollectionMsg >> dispatch

    let agedEntriesWithInfo =
        entries
        |> List.map (AgedEntryWithInfo.fromEntry cardInfo)
        |> List.choose id

    Components.Collection.View.render prefs dsEntries agedEntriesWithInfo state.collection dispatch

let renderInventoryView state dispatch =
    let prefs = state ^. StateLenses.prefs
    let entries = state ^. StateLenses.entries
    let setData = state ^. StateLenses.setData
    let cardInfo = state ^. StateLenses.cardInfo
    let dispatch = InventoryMsg >> dispatch
    Components.Inventory.View.render prefs cardInfo entries state.inventory dispatch

let createTab (header: string) (content: IView) =
    let content =
        Border.create [
            Border.borderThickness (0., 1., 0., 0.)
            Border.borderBrush Config.lineColor
            Border.child content
        ]

    TabItem.create [
        TabItem.fontSize 22.
        TabItem.header header
        TabItem.content content
    ]

let createTabs state dispatch : IView list =
    [
        createTab "Collection" (renderCollectionView state dispatch)
        createTab "Analyse" (AnalyseView.render state dispatch)
        createTab "Inventory" (renderInventoryView state dispatch)
        createTab "Preferences" (PreferenceView.render state dispatch)
    ]

let render (state: State) (dispatch: Dispatch) : IView =
    TabControl.create [
        TabControl.tabStripPlacement Dock.Top // Change this property to tell the app where to show the tab bar
        TabControl.viewItems (createTabs state dispatch)
    ]
    :> IView
