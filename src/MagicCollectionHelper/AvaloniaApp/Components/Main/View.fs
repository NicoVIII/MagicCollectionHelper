module MagicCollectionHelper.AvaloniaApp.Components.Main.View

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Controls.Primitives

let sideBarButton (label: string) (msg: Msg) (enabled: bool) (dispatch: Dispatch) =
    Button.create [
        Button.content label
        Button.isEnabled enabled
        Button.padding (20., 10.)
        Button.onClick (fun _ -> msg |> dispatch)
    ]

let sideBar (state: State) (dispatch: Dispatch): IView =
    StackPanel.create [
        StackPanel.dock Dock.Left
        StackPanel.orientation Orientation.Vertical
        StackPanel.children [
            sideBarButton "Import" ImportFromFile true dispatch
            sideBarButton "Analyse" Analyse (not state.cards.IsEmpty) dispatch
        ]
    ]
    :> IView

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
        Border.borderBrush "lightgray"
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
            ScrollViewer.create [
                ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                ScrollViewer.padding 10.
                ScrollViewer.content (
                    TextBlock.create [
                        TextBlock.fontSize 12.
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.textAlignment TextAlignment.Center
                        TextBlock.textWrapping TextWrapping.Wrap
                        TextBlock.text state.text
                    ]
                )
            ]
        ]
    ]
    :> IView
