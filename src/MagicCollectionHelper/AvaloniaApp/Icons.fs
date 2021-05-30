namespace MagicCollectionHelper.AvaloniaApp

open Avalonia.Controls
open Avalonia.Controls.Shapes
open Avalonia.FuncUI.DSL

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

    // From https://materialdesignicons.com/icon/reload
    let reload =
        createWithClasses
            [ "rotatingCW" ]
            "M2 12C2 16.97 6.03 21 11 21C13.39 21 15.68 20.06 17.4 18.4\
            L15.9 16.9C14.63 18.25 12.86 19 11 19C4.76 19 1.64 11.46 6.05 7.05\
            C10.46 2.64 18 5.77 18 12H15L19 16H19.1L23 12H20\
            C20 7.03 15.97 3 11 3C6.03 3 2 7.03 2 12Z"

    // From https://materialdesignicons.com/icon/reload-alert
    let reloadAlert =
        create
            "M2 12C2 17 6 21 11 21C13.4 21 15.7 20.1 17.4 18.4L15.9 16.9\
            C14.6 18.3 12.9 19 11 19C4.8 19 1.6 11.5 6.1 7.1S18 5.8 18 12\
            H15L19 16H19.1L23 12H20C20 7 16 3 11 3S2 7 2 12M10 15H12V17\
            H10V15M10 7H12V13H10V7"

    // From https://materialdesignicons.com/icon/sync
    let sync =
        createWithClasses
            [ "rotatingCCW" ]
            "M12,18A6,6 0 0,1 6,12C6,11 6.25,10.03 6.7,9.2L5.24,7.74\
            C4.46,8.97 4,10.43 4,12A8,8 0 0,0 12,20V23L16,19L12,15M12,4V1L8,5\
            L12,9V6A6,6 0 0,1 18,12C18,13 17.75,13.97 17.3,14.8L18.76,16.26\
            C19.54,15.03 20,13.57 20,12A8,8 0 0,0 12,4Z"

    // From https://materialdesignicons.com/icon/check
    let check =
        create "M21,7L9,19L3.5,13.5L4.91,12.09L9,16.17L19.59,5.59L21,7Z"
