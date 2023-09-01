namespace MagicCollectionHelper.AvaloniaApp

open Elmish
open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.Hosts
open Avalonia.Themes.Fluent

open MagicCollectionHelper.AvaloniaApp.Main

type MainWindow() as this =
    inherit HostWindow()

    do
#if DEBUG
        this.AttachDevTools()
#endif

        base.Title <- "MagicCollectionHelper"
        base.Width <- 800.0
        base.Height <- 600.0

        this.Content <- Main.view

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())
        this.Styles.Load "avares://MagicCollectionHelper.AvaloniaApp/Styles.xaml"
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =
    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
