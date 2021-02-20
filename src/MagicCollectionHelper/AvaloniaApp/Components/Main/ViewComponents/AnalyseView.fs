namespace MagicCollectionHelper.AvaloniaApp.Components.Main.ViewComponents

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Main
open MagicCollectionHelper.AvaloniaApp.Elements

module AnalyseView =
    let topBar (state: State) (dispatch: Dispatch): IView =
        ActionButtonBar.create [
            ActionButton.create {
                text = "Analyse"
                isEnabled = (not state.cards.IsEmpty)
                action = (fun _ -> Analyse |> dispatch)
            }
        ]

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
