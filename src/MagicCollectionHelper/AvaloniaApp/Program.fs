namespace MagicCollectionHelper.AvaloniaApp

open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Diagnostics
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Components.Hosts
open Avalonia.Input

open MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated

module CollectionMsg = Msg

open MagicCollectionHelper.AvaloniaApp.Main
open MagicCollectionHelper.AvaloniaApp.Main.Generated

type MainWindow() as this =
    inherit HostWindow()

#if DEBUG
    let msgToString msg =
        match msg with
        | CollectionMsg cMsg -> $"{Msg.toString msg}: {CollectionMsg.toString cMsg}"
        | msg -> Msg.toString msg
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

        Program.mkProgram Main.Model.init Main.Update.perform Main.View.render
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
