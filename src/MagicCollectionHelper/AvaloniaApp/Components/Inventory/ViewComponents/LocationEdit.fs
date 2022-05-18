namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory.ViewComponents

open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media

open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp.Components.Inventory

module LocationEdit =
    let inSetLine _ _ inSet =
        let valueControl =
            TextBlock.create [
                TextBlock.text (
                    inSet
                    |> Set.map MagicSet.unwrap
                    |> Set.toList
                    |> String.concat ","
                )
            ]

        ("In set", valueControl)

    let isFoilLine dispatch locationName isFoil =
        let valueControl =
            CheckBox.create [
                CheckBox.isChecked (Some isFoil)
                CheckBox.onChecked (
                    (fun _ ->
                        UpdateLocationRules(locationName, Rules.withIsFoil true)
                        |> dispatch),
                    OnChangeOf locationName
                )
                CheckBox.onUnchecked (
                    (fun _ ->
                        UpdateLocationRules(locationName, Rules.withIsFoil false)
                        |> dispatch),
                    OnChangeOf locationName
                )
            ]

        ("Is foil", valueControl)

    let inLanguageLine dispatch locationName inLanguage =
        let valueControl =
            TextBox.create [
                TextBox.text (string inLanguage)
                TextBox.onTextChanged (fun lang ->
                    UpdateLocationRules(locationName, Rules.withInLanguage (Language lang))
                    |> dispatch)
            ]

        ("In language", valueControl)

    let limitLine dispatch locationName (limit: uint) =
        let valueControl =
            NumericUpDown.create [
                NumericUpDown.maximum 99999.
                NumericUpDown.minimum 1.
                NumericUpDown.value (float limit)
                NumericUpDown.onValueChanged (
                    (fun v ->
                        UpdateLocationRules(locationName, Rules.withLimit (uint v))
                        |> dispatch),
                    OnChangeOf locationName
                )
            ]

        ("Limit", valueControl)

    let limitExactLine dispatch locationName (limitExact: uint) =
        let valueControl =
            NumericUpDown.create [
                NumericUpDown.maximum 99999.
                NumericUpDown.minimum 1.
                NumericUpDown.value (float limitExact)
                NumericUpDown.onValueChanged (
                    (fun v ->
                        UpdateLocationRules(locationName, Rules.withLimitExact (uint v))
                        |> dispatch),
                    OnChangeOf locationName
                )
            ]

        ("Limit (exact)", valueControl)

    let renderRule dispatch locationName filterLine (option: 'a option) =
        match option with
        | Some value ->
            let (title, control) = filterLine dispatch locationName value

            StackPanel.create [
                StackPanel.orientation Orientation.Horizontal
                StackPanel.spacing 10.
                StackPanel.children [
                    TextBlock.create [
                        TextBlock.column 0
                        TextBlock.row 0
                        TextBlock.width 120.
                        TextBlock.text title
                    ]
                    control
                ]
            ]
            :> IView
        | None -> TextBlock.create [] :> IView

    let renderLocationLine (location: CustomLocation) (dispatch: Dispatch) : IView =
        let renderRule filterLine =
            renderRule dispatch location.name filterLine

        let rules = location.rules

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
                        renderRule inSetLine rules.inSet
                        renderRule isFoilLine rules.isFoil
                        renderRule inLanguageLine rules.inLanguage
                        renderRule limitLine rules.limit
                        renderRule limitExactLine rules.limitExact
                    ]
                ]
            )
        ]
        :> IView

    let render (state: State) (dispatch: Dispatch) : IView =
        StackPanel.create [
            StackPanel.orientation Orientation.Vertical
            StackPanel.children [
                for location in state.locations do
                    Expander.create [
                        Expander.header location.name
                        Expander.content (renderLocationLine location dispatch)
                    ]
            ]
        ]
        :> IView
