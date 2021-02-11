module MagicCollectionHelper.AvaloniaApp.Components.Main.View

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Main.ViewComponents

let sideBarButton (label: string) (msg: Msg) (dispatch: Dispatch) =
    Button.create [
        Button.content label
        Button.padding (40., 14.)
        Button.onClick (fun _ -> msg |> dispatch)
    ]

let sideBar (state: State) (dispatch: Dispatch): IView =
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
                        StackPanel.children [
                            sideBarButton "Preferences" (ChangeViewMode Preferences) dispatch
                        ]
                    ]
                    StackPanel.create [
                        StackPanel.dock Dock.Top
                        StackPanel.orientation Orientation.Vertical
                        StackPanel.children [
                            sideBarButton "Import" Import dispatch
                            sideBarButton "Analyse" (ChangeViewMode ViewMode.Analyse) dispatch
                        ]
                    ]
                ]
            ])
    ]
    :> IView

let content (state: State) (dispatch: Dispatch): IView =
    match state.viewMode with
    | Start ->
        Border.create [
            Border.padding 10.
            Border.child (
                TextBlock.create [
                    TextBlock.fontSize 16.
                    TextBlock.textAlignment TextAlignment.Center
                    TextBlock.textWrapping TextWrapping.Wrap
                    TextBlock.text Config.startText
                ]
            )
        ]
        :> IView
    | ViewMode.Analyse ->
        AnalyseView.render state dispatch
    | Preferences ->
        PreferenceView.render state dispatch

let leftBottomBar (state: State) (dispatch: Dispatch): IView =
    StackPanel.create [
        StackPanel.dock Dock.Left
        StackPanel.orientation Orientation.Horizontal
        StackPanel.children [
            TextBlock.create [
                TextBlock.text $"Loaded entries: %i{state.cards.Length}"
            ]
        ]
    ]
    :> IView

let bottomBar (state: State) (dispatch: Dispatch): IView =
    Border.create [
        Border.dock Dock.Bottom
        Border.borderBrush Config.lineColor
        Border.borderThickness (0., 1., 0., 0.)
        Border.child (
            DockPanel.create [
                DockPanel.margin 5.0
                DockPanel.children [
                    leftBottomBar state dispatch
                    StackPanel.create [
                        StackPanel.dock Dock.Right
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.children []
                    ]
                ]
            ]
        )
    ]
    :> IView

let render (state: State) (dispatch: Dispatch): IView =
    DockPanel.create [
        DockPanel.children [
            bottomBar state dispatch
            sideBar state dispatch
            content state dispatch
        ]
    ]
    :> IView
