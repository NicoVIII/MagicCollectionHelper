namespace MagicCollectionHelper.AvaloniaApp.Elements

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout

open MagicCollectionHelper.AvaloniaApp

type ActionButtonProps =
    { action: Avalonia.Interactivity.RoutedEventArgs -> unit
      isEnabled: bool
      subPatch: SubPatchOptions
      text: string }

module ActionButton =
    let create props =
        Button.create [
            Button.content props.text
            Button.isEnabled props.isEnabled
            Button.padding (20., 7.)
            Button.onClick (props.action, props.subPatch)
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

module TabView =
    type TabPlacement =
        | Top
        | Left
        | Right
        | Bottom

    let render renderTab dispatchMsg placement tabs content current =
        let tabContainer =
            let dock =
                match placement with
                | Top -> Dock.Top
                | Left -> Dock.Left
                | Right -> Dock.Right
                | Bottom -> Dock.Bottom

            let orientation =
                match placement with
                | Left
                | Right -> Orientation.Vertical
                | Top
                | Bottom -> Orientation.Horizontal

            ScrollViewer.create [
                ScrollViewer.dock dock
                match placement with
                | Left
                | Right -> ScrollViewer.verticalScrollBarVisibility ScrollBarVisibility.Hidden
                | Top
                | Bottom -> ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Hidden
                ScrollViewer.content (
                    StackPanel.create [
                        StackPanel.orientation orientation
                        StackPanel.children [
                            for tab in tabs do
                                let tabText : string = renderTab tab

                                ToggleButton.create [
                                    ToggleButton.content tabText
                                    ToggleButton.isChecked (tab = current |> Some)
                                    ToggleButton.onClick ((fun _ -> tab |> dispatchMsg), OnChangeOf current)
                                ]
                        ]
                    ]
                )
            ]
            :> IView

        DockPanel.create [
            DockPanel.children [
                tabContainer
                content
            ]
        ]
        :> IView

    /// Does render a tab view from a map. It has to be provided as list because
    /// order matters in this context
    let renderFromList renderTab renderTabData dispatchMsg placement list current =
        let tabs, content =
            List.foldBack
                (fun (tab, tabData) (tabs, content) ->
                    let tabs = tab :: tabs

                    let content =
                        if current = tab then
                            renderTabData tab tabData |> Some
                        else
                            content

                    tabs, content)
                list
                ([], None)

        render renderTab dispatchMsg placement tabs content.Value current
