namespace MagicCollectionHelper.AvaloniaApp.Main.Ready.ViewComponents

open Avalonia.Controls
open Avalonia.Layout
open SimpleOptics

open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp.Main.Ready
open MagicCollectionHelper.AvaloniaApp.ViewHelper

module PreferenceView =
    let numInputProps dispatch (min: uint, max: uint) lens prefs =
        let currentValue: uint = Optic.get lens prefs

        [
            NumericUpDown.margin (10., 0., 0., 0.)
            NumericUpDown.maximum max
            NumericUpDown.minimum min
            NumericUpDown.value currentValue
            NumericUpDown.onValueChanged (
                (fun newValue ->
                    let newValue = uint newValue.Value

                    if currentValue <> newValue then
                        newValue |> Optic.set lens |> ChangePrefs |> dispatch),
                OnChangeOf currentValue
            )
        ]

    let withTableProps row column attrList =
        [ NumericUpDown.column column; NumericUpDown.row row ] |> List.append attrList

    let render (state: State) (dispatch: Dispatch) : IView =
        let numInputProps = numInputProps dispatch

        let prefs = Optic.get StateOptic.prefs state

        Border.create [
            Border.padding 10.
            Border.child (
                StackPanel.create [
                    StackPanel.spacing 5.
                    StackPanel.children [
                        TextBlock.create [
                            TextBlock.text
                                "All numbers in the preferences use base ten, indipendent of the number base setting."
                        ]
                        Grid.create [
                            Grid.columnDefinitions "Auto, Auto"
                            Grid.rowDefinitions (List.replicate 3 "Auto" |> String.concat ",5,")
                            Grid.children [
                                label 0 "Card grouping sizes"
                                StackPanel.create [
                                    StackPanel.row 0
                                    StackPanel.column 1
                                    StackPanel.orientation Orientation.Horizontal
                                    StackPanel.children [
                                        numInputProps (0u, 200u) PrefsOptic.cardGroupMinSize prefs
                                        |> NumericUpDown.create

                                        numInputProps (0u, 200u) PrefsOptic.cardGroupMaxSize prefs
                                        |> NumericUpDown.create
                                    ]
                                ]
                                label 2 "Missing from percent"
                                NumericUpDown.create [
                                    NumericUpDown.column 1
                                    NumericUpDown.margin (10., 0., 0., 0.)
                                    NumericUpDown.maximum 100
                                    NumericUpDown.minimum 0
                                    NumericUpDown.row 2
                                    NumericUpDown.value (uint (prefs.missingPercent * 100m))
                                    NumericUpDown.onValueChanged (
                                        (fun value ->
                                            if value.Value <> decimal (prefs.missingPercent * 100m) then
                                                value.Value / 100m
                                                |> Optic.set PrefsOptic.missingPercent
                                                |> ChangePrefs
                                                |> dispatch),
                                        OnChangeOf prefs.missingPercent
                                    )
                                ]
                                label 4 "Number base"
                                ComboBox.create [
                                    ComboBox.column 1
                                    ComboBox.row 4
                                    ComboBox.dataItems [ Decimal; Dozenal; Seximal ]
                                    ComboBox.selectedItem prefs.numBase
                                    ComboBox.itemTemplate (
                                        DataTemplateView<NumBase>.create
                                            (fun numBase ->
                                                TextBlock.create [ TextBlock.text (NumBase.toString numBase) ])
                                    )
                                    ComboBox.onSelectedItemChanged (fun v ->
                                        if v <> null then
                                            v
                                            |> unbox<NumBase>
                                            |> Optic.set PrefsOptic.numBase
                                            |> ChangePrefs
                                            |> dispatch)
                                ]
                            ]
                        ]
                        CheckBox.create [
                            CheckBox.content "Include Foils in Set Analyser"
                            CheckBox.isChecked prefs.setWithFoils
                            CheckBox.onChecked (
                                (fun _ -> (Optic.set PrefsOptic.setWithFoils true) |> ChangePrefs |> dispatch),
                                Never
                            )
                            CheckBox.onUnchecked (
                                (fun _ -> (Optic.set PrefsOptic.setWithFoils false) |> ChangePrefs |> dispatch),
                                Never
                            )
                        ]
                    ]
                ]
            )
        ]
        :> IView
