﻿namespace MagicCollectionHelper.AvaloniaApp

open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Diagnostics
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Components.Hosts

open MagicCollectionHelper.AvaloniaApp.Components
open Avalonia.Input

type MainWindow() as this =
    inherit HostWindow()

    do
        base.Title <- "MagicCollectionHelper"
        base.Width <- 800.0
        base.Height <- 600.0

        //this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
        //this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true
#if DEBUG
        DevTools.Attach(this, KeyGesture(Key.F12))
        |> ignore
#endif

        Program.mkProgram Main.Model.init Main.Update.perform Main.View.render
        |> Program.withHost this
#if DEBUG
        |> Program.withTrace
            (fun msg state ->
                printfn "Got message: %A" msg

                match msg with
                | Main.InventoryMsg (Inventory.UpdateLocationRules (locationName, _)) ->
                    let location =
                        Map.find locationName state.inventory.locations

                    printfn "Updated location rules: %A" location.rules
                | _ -> ())
#endif
        |> Program.run

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
        this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main (args: string []) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
