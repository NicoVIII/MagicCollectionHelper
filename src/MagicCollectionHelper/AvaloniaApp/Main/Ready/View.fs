module MagicCollectionHelper.AvaloniaApp.Main.Ready.View

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Main.Ready.ViewComponents

let sideBarButton currentViewMode (label: string) viewMode (dispatch: Dispatch) =
    let isActive = currentViewMode = viewMode

    ToggleButton.create [
        ToggleButton.isChecked isActive
        ToggleButton.content label
        ToggleButton.padding (40., 14.)
        ToggleButton.onClick (fun _ -> viewMode |> ChangeViewMode |> dispatch)
    ]

let sideBar (state: State) (dispatch: Dispatch) : IView =
    let sideBarButton =
        sideBarButton (getl StateLenses.viewMode state)

    Border.create [
        Border.dock Dock.Left
        Border.borderBrush Config.lineColor
        Border.borderThickness (0., 0., 1., 0.)
        Border.child (
            DockPanel.create [
                DockPanel.children [
                    StackPanel.create [
                        StackPanel.dock Dock.Bottom
                        StackPanel.orientation Orientation.Vertical
                        StackPanel.spacing 1.
                        StackPanel.children [
                            sideBarButton "Preferences" Preferences dispatch
                        ]
                    ]
                    StackPanel.create [
                        StackPanel.orientation Orientation.Vertical
                        StackPanel.spacing 1.
                        StackPanel.children [
                            sideBarButton "Collection" Collection dispatch
                            sideBarButton "Analyse" ViewMode.Analyse dispatch
                            sideBarButton "Inventory" Inventory dispatch
                        ]
                    ]
                ]
            ]
        )
    ]
    :> IView

let content (state: State) (dispatch: Dispatch) : IView =
    let cardInfo = getl StateLenses.cardInfo state

    match getl StateLenses.viewMode state with
    | Collection ->
        let dsEntries = getl StateLenses.dsEntries state
        let entries = getl StateLenses.entries state
        let dispatch = CollectionMsg >> dispatch

        Components.Collection.View.render dsEntries entries cardInfo state.collection dispatch
    | ViewMode.Analyse -> AnalyseView.render state dispatch
    | Inventory ->
        let entries = getl StateLenses.entries state
        let setData = getl StateLenses.setData state
        let cardInfo = getl StateLenses.cardInfo state
        let dispatch = InventoryMsg >> dispatch
        Components.Inventory.View.render cardInfo setData entries state.inventory dispatch
    | Preferences -> PreferenceView.render state dispatch

let leftBottomBar (state: State) (dispatch: Dispatch) : IView =
    let entries = getl StateLenses.dsEntries state

    StackPanel.create [
        StackPanel.dock Dock.Left
        StackPanel.orientation Orientation.Horizontal
        StackPanel.children [
            TextBlock.create [
                TextBlock.text $"Loaded entries: %i{entries.Length}"
            ]
        ]
    ]
    :> IView

let render (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            sideBar state dispatch
            content state dispatch
        ]
    ]
    :> IView
