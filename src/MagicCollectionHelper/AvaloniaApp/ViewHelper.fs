namespace MagicCollectionHelper.AvaloniaApp

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout

module ViewHelper =
    let label row label =
        TextBlock.create [
            TextBlock.column 0
            TextBlock.row row
            TextBlock.text label
            TextBlock.verticalAlignment VerticalAlignment.Center
        ]

module String =
    let iContains (s1: string) (s2: string) = (s1.ToLower()).Contains(s2.ToLower())
