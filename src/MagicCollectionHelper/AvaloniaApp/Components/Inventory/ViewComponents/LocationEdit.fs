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
    let renderRuleLine possibleRuleTypes (rule: Rule) =
        StackPanel.create [
            StackPanel.orientation Orientation.Horizontal
            StackPanel.spacing 10.
            StackPanel.children [
                ComboBox.create [
                    ComboBox.column 0
                    ComboBox.dataItems possibleRuleTypes
                    ComboBox.row 0
                    ComboBox.selectedItem (DUs.Rule.toString rule)
                    ComboBox.width 120.
                ]
                match rule with
                | InSet v ->
                    TextBox.create [
                        TextBox.text v.Value
                        TextBox.width 70.
                    ]
                | InLanguage v ->
                    TextBox.create [
                        TextBox.text v.Value
                        TextBox.width 70.
                    ]
                | IsFoil v ->
                    CheckBox.create [
                        CheckBox.isChecked v
                    ]
                | Limit v ->
                    NumericUpDown.create [
                        NumericUpDown.maximum 99999.
                        NumericUpDown.minimum 1.
                        NumericUpDown.value (float v)
                    ]
            ]
        ]

    let renderLocationLine (location: CustomLocation) (dispatch: Dispatch): IView =
        let possibleRuleTypes =
            typeof<Rule>
            |> FSharpType.GetUnionCases
            |> Array.map (fun case -> case.Name)
        let renderRuleLine = renderRuleLine possibleRuleTypes

        Border.create [
            Border.padding (20., 10.)
            Border.child(
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.spacing 5.
                    StackPanel.children [
                        TextBlock.create [
                            TextBlock.fontSize 14.
                            TextBlock.fontWeight FontWeight.Bold
                            TextBlock.text "Rules"
                        ]
                        for rule in location.rules do
                            renderRuleLine rule
                    ]
                ])
        ] :> IView

    let render (state: State) (dispatch: Dispatch): IView =
        StackPanel.create [
            StackPanel.orientation Orientation.Vertical
            StackPanel.children [
                for location in state.locations do
                    Expander.create [
                        Expander.header location.name
                        Expander.content (renderLocationLine location dispatch)
                    ]
            ]
        ] :> IView
