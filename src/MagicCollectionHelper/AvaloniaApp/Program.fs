namespace MagicCollectionHelper.AvaloniaApp

open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Diagnostics
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Components.Hosts
open Avalonia.Input

open MagicCollectionHelper.AvaloniaApp.Main
open MagicCollectionHelper.AvaloniaApp.Components

type MainWindow() as this =
    inherit HostWindow()

#if DEBUG
    let msgToString msg =
        match msg with
        | LoadingMsg subMsg ->
            $"{Generated.Msg.toString msg}-{Loading.Generated.Msg.toString subMsg}"
        | ReadyMsg subMsg ->
            let subSubMsgString =
                match subMsg with
                | Ready.Msg.InventoryMsg subSubMsg ->
                    $"{Inventory.Generated.Msg.toString subSubMsg}"
                | _ -> "???"

            $"{Generated.Msg.toString msg}-{Ready.Generated.Msg.toString subMsg}-{subSubMsgString}"
#endif

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

        Program.mkProgram Model.init Update.perform View.render
        |> Program.withHost this
#if DEBUG
        |> Program.withTrace (fun msg _ -> printfn "Got message: %s" (msgToString msg))
#endif
        |> Program.run

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Load "avares://Avalonia.Themes.Default/DefaultTheme.xaml"
        this.Styles.Load "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml"
        this.Styles.Load "avares://MagicCollectionHelper.AvaloniaApp/Styles.xaml"

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main (args: string []) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
