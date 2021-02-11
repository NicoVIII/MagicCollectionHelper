namespace MagicCollectionHelper.AvaloniaApp.Components.Main.ViewComponents

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Main

module AnalyseView =
    let topBar (state: State) (dispatch: Dispatch): IView =
        Border.create [
            Border.dock Dock.Top
            Border.borderThickness (0.,0.,0.,1.)
            Border.borderBrush Config.lineColor
            Border.child(
                StackPanel.create [
                    StackPanel.orientation Orientation.Horizontal
                    StackPanel.children [
                        Button.create [
                            Button.content "Analyse"
                            Button.isEnabled  (not state.cards.IsEmpty)
                            Button.padding (20.,7.)
                            Button.onClick (fun _ -> Analyse |> dispatch)
                        ]
                    ]
                ]
            )
        ] :> IView

    let content (state: State) (dispatch: Dispatch): IView =
        Border.create [
            Border.background "black"
            Border.child (
                ScrollViewer.create [
                    ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                    ScrollViewer.padding 10.
                    ScrollViewer.content (
                        TextBlock.create [
                            TextBlock.fontFamily Config.monospaceFont
                            TextBlock.fontSize 12.
                            TextBlock.textWrapping TextWrapping.Wrap
                            TextBlock.text state.analyseText
                        ]
                    )
                ])
        ]:> IView

    let render (state: State) (dispatch: Dispatch): IView =
        DockPanel.create [
            DockPanel.children [
                topBar state dispatch
                content state dispatch
            ]
        ]:> IView
