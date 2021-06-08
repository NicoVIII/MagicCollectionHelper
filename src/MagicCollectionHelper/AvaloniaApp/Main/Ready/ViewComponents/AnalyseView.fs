namespace MagicCollectionHelper.AvaloniaApp.Main.Ready.ViewComponents

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Media

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Elements
open MagicCollectionHelper.AvaloniaApp.Main.Ready

module AnalyseView =
    let topBar (state: State) (dispatch: Dispatch) : IView =
        let entries = getl StateLenses.dsEntries state

        ActionButtonBar.create [
            ActionButton.create
                { text = "Analyse"
                  isEnabled = (not entries.IsEmpty)
                  action = (fun _ -> Analyse |> dispatch)
                  subPatch = Never }
        ]

    let content (state: State) (dispatch: Dispatch) : IView =
        let analyseText = getl StateLenses.analyseText state

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
                            TextBlock.text analyseText
                        ]
                    )
                ]
            )
        ]
        :> IView

    let render (state: State) (dispatch: Dispatch) : IView =
        DockPanel.create [
            DockPanel.children [
                topBar state dispatch
                content state dispatch
            ]
        ]
        :> IView
