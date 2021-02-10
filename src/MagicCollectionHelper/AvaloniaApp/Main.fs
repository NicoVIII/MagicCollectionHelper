﻿namespace MagicCollectionHelper.AvaloniaApp

module Main =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.Media

    type State = unit
    let init = ()

    type Msg = unit

    let update (msg: Msg) (state: State): State = state

    let view (state: State) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                StackPanel.create [
                    StackPanel.dock Dock.Left
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.children []
                ]
                TextBlock.create [
                    TextBlock.fontSize 18.0
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.horizontalAlignment HorizontalAlignment.Center
                    TextBlock.textAlignment TextAlignment.Center
                    TextBlock.textWrapping TextWrapping.Wrap
                    TextBlock.text ("This is a placeholder text! This is the first, very basic version of a GUI.")
                ]
            ]
        ]