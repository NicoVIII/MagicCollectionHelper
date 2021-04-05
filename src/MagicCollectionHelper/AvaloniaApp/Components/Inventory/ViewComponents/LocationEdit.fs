namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory.ViewComponents

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open Microsoft.FSharp.Reflection

open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.ViewHelper
open MagicCollectionHelper.Core.Types

module LocationEdit =
    let renderRuleLine text valueControl =
        StackPanel.create [
            StackPanel.orientation Orientation.Horizontal
            StackPanel.spacing 10.
            StackPanel.children [
                TextBlock.create [
                    TextBlock.column 0
                    TextBlock.row 0
                    TextBlock.width 120.
                    TextBlock.text text
                ]
                valueControl
            ]
        ]

    let renderIsFoilRuleLine isFoil =
        let valueControl =
            CheckBox.create [
                CheckBox.isChecked (Some isFoil)
            ]

        renderRuleLine "Is foil" valueControl

    let renderLimitRuleLine dispatch location limit =
        let valueControl =
            NumericUpDown.create [
                NumericUpDown.maximum 99999.
                NumericUpDown.minimum 1.
                NumericUpDown.value (float limit)
                NumericUpDown.onValueChanged (
                    (fun v ->
                        let rules =
                            { location.rules with
                                  limit = Some(uint v) }

                        UpdateLocationRules(location.name, rules)
                        |> dispatch),
                    OnChangeOf(location.name, location.rules)
                )
            ]

        renderRuleLine "Limit" valueControl

    let renderLimitExactRuleLine limitExact =
        let valueControl =
            NumericUpDown.create [
                NumericUpDown.maximum 99999.
                NumericUpDown.minimum 1.
                NumericUpDown.value (float limitExact)
            (*NumericUpDown.onValueChanged
                    (fun v ->
                        let rule = LimitExact(uint v)
                        UpdateLocationRule(locationName, rule) |> dispatch)*)
            ]

        renderRuleLine "Limit (exact)" valueControl

    let renderLocationLine (location: CustomLocation) (dispatch: Dispatch) : IView =
        Border.create [
            Border.padding (20., 10.)
            Border.child (
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.spacing 5.
                    StackPanel.children [
                        TextBlock.create [
                            TextBlock.fontSize 14.
                            TextBlock.fontWeight FontWeight.Bold
                            TextBlock.verticalAlignment VerticalAlignment.Center
                            TextBlock.text "Rules"
                        ]
                        match location.rules.isFoil with
                        | Some isFoil -> renderIsFoilRuleLine isFoil
                        | None -> ()
                        match location.rules.limit with
                        | Some limit -> renderLimitRuleLine dispatch location limit
                        | None -> ()
                        match location.rules.limitExact with
                        | Some limitExact -> renderLimitExactRuleLine limitExact
                        | None -> ()
                    ]
                ]
            )
        ]
        :> IView

    let render (state: State) (dispatch: Dispatch) : IView =
        StackPanel.create [
            StackPanel.orientation Orientation.Vertical
            StackPanel.children [
                for KeyValue (name, location) in state.locations do
                    Expander.create [
                        Expander.header name
                        Expander.content (renderLocationLine location dispatch)
                    ]
            ]
        ]
        :> IView
