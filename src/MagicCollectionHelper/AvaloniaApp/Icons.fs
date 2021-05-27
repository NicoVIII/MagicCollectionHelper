namespace MagicCollectionHelper.AvaloniaApp

open Avalonia.Controls
open Avalonia.Controls.Shapes
open Avalonia.FuncUI.DSL
open Avalonia.Media

module Icons =
    let createWithClasses (classes: seq<string>) (data: string) =
        Canvas.create [
            Canvas.width 24.0
            Canvas.height 24.0
            Canvas.classes [
                "icon"
                for clas in classes do
                    clas
            ]
            Canvas.children [
                Path.create [ Path.data data ]
            ]
        ]

    let create = createWithClasses []

    // From https://materialdesignicons.com/icon/sync
    let sync =
        createWithClasses
            [ "rotatingCCW" ]
            "M12,18A6,6 0 0,1 6,12C6,11 6.25,10.03 6.7,9.2L5.24,7.74C4.46,8.97 4,\
            10.43 4,12A8,8 0 0,0 12,20V23L16,19L12,15M12,4V1L8,5L12,9V6A6,6 0 0,\
            1 18,12C18,13 17.75,13.97 17.3,14.8L18.76,16.26C19.54,15.03 20,13.57 20,\
            12A8,8 0 0,0 12,4Z"

    // From https://materialdesignicons.com/icon/check
    let check =
        create "M21,7L9,19L3.5,13.5L4.91,12.09L9,16.17L19.59,5.59L21,7Z"
