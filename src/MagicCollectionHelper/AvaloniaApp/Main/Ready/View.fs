module MagicCollectionHelper.AvaloniaApp.Main.Ready.View

open Avalonia.Controls

open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Main.Ready.ViewComponents

let renderCollectionView state dispatch =
    let cardInfo = getl StateLenses.cardInfo state
    let dsEntries = getl StateLenses.dsEntries state
    let entries = getl StateLenses.entries state
    let dispatch = CollectionMsg >> dispatch

    Components.Collection.View.render dsEntries entries cardInfo state.collection dispatch

let renderInventoryView state dispatch =
    let entries = getl StateLenses.entries state
    let setData = getl StateLenses.setData state
    let cardInfo = getl StateLenses.cardInfo state
    let dispatch = InventoryMsg >> dispatch
    Components.Inventory.View.render cardInfo setData entries state.inventory dispatch

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
    [ createTab "Collection" (renderCollectionView state dispatch)
      createTab "Analyse" (AnalyseView.render state dispatch)
      createTab "Inventory" (renderInventoryView state dispatch)
      createTab "Preferences" (PreferenceView.render state dispatch) ]

let render (state: State) (dispatch: Dispatch) : IView =
    TabControl.create [
        TabControl.tabStripPlacement Dock.Top // Change this property to tell the app where to show the tab bar
        TabControl.viewItems (createTabs state dispatch)
    ]
    :> IView
