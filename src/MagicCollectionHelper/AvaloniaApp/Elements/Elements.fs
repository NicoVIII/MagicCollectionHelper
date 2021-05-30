namespace MagicCollectionHelper.AvaloniaApp.Elements

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout

open MagicCollectionHelper.AvaloniaApp

type ActionButtonProps =
    { action: Avalonia.Interactivity.RoutedEventArgs -> unit
      isEnabled: bool
      text: string }

module ActionButton =
    let create props =
        Button.create [
            Button.content props.text
            Button.isEnabled props.isEnabled
            Button.padding (20., 7.)
            Button.onClick props.action
        ]

module ActionButtonBar =
    let create buttonList =
        Border.create [
            Border.dock Dock.Right
            Border.borderThickness (1., 0., 0., 0.)
            Border.borderBrush Config.lineColor
            Border.child (
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.spacing 1.
                    StackPanel.children buttonList
                ]
            )
        ]
        :> IView
