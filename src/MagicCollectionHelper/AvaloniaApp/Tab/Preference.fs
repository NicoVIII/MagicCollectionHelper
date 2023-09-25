namespace MagicCollectionHelper.AvaloniaApp.Tab

open Avalonia.Controls
open Avalonia.Layout
open SimpleOptics

open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp.ViewHelper

[<RequireQualifiedAccess>]
module Preference =
    let numInputProps (prefs: IWritable<Prefs>) (min: uint, max: uint) lens =
        let currentValue: uint = Optic.get lens prefs.Current

        [
            NumericUpDown.margin (10., 0., 0., 0.)
            NumericUpDown.maximum max
            NumericUpDown.minimum min
            NumericUpDown.value currentValue
            NumericUpDown.onValueChanged (
                (fun newValue ->
                    let newValue = uint newValue.Value

                    if currentValue <> newValue then
                        Optic.set lens newValue prefs.Current |> prefs.Set),
                OnChangeOf currentValue
            )
        ]

    let withTableProps row column attrList =
        [ NumericUpDown.column column; NumericUpDown.row row ] |> List.append attrList

    let render (prefs: IWritable<Prefs>) : IView =
        let numInputProps = numInputProps prefs

        Border.create [
            Border.padding 10.
            Border.child (
                StackPanel.create [
                    StackPanel.spacing 5.
                    StackPanel.children [
                        TextBlock.create [
                            TextBlock.text
                                "All numbers in the preferences use base ten, independent of the number base setting."
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
                                        numInputProps (0u, 200u) PrefsOptic.cardGroupMinSize |> NumericUpDown.create
                                        numInputProps (0u, 200u) PrefsOptic.cardGroupMaxSize |> NumericUpDown.create
                                    ]
                                ]
                                label 2 "Missing from percent"
                                NumericUpDown.create [
                                    NumericUpDown.column 1
                                    NumericUpDown.margin (10., 0., 0., 0.)
                                    NumericUpDown.maximum 100
                                    NumericUpDown.minimum 0
                                    NumericUpDown.row 2
                                    NumericUpDown.value (uint (prefs.Current.missingPercent * 100m))
                                    NumericUpDown.onValueChanged (
                                        (fun value ->
                                            if value.Value <> decimal (prefs.Current.missingPercent * 100m) then
                                                Optic.set PrefsOptic.missingPercent (value.Value / 100m) prefs.Current
                                                |> prefs.Set),
                                        OnChangeOf prefs.Current.missingPercent
                                    )
                                ]
                                label 4 "Number base"
                                ComboBox.create [
                                    ComboBox.column 1
                                    ComboBox.row 4
                                    ComboBox.dataItems [ Decimal; Dozenal; Seximal ]
                                    ComboBox.selectedItem prefs.Current.numBase
                                    ComboBox.itemTemplate (
                                        DataTemplateView<NumBase>.create
                                            (fun numBase ->
                                                TextBlock.create [ TextBlock.text (NumBase.toString numBase) ])
                                    )
                                    ComboBox.onSelectedItemChanged (fun v ->
                                        if v <> null then
                                            Optic.set PrefsOptic.numBase (unbox<NumBase> v) prefs.Current |> prefs.Set)
                                ]
                            ]
                        ]
                        CheckBox.create [
                            CheckBox.content "Include Foils in Set Analyser"
                            CheckBox.isChecked prefs.Current.setWithFoils
                            CheckBox.onChecked (
                                (fun _ -> Optic.set PrefsOptic.setWithFoils true prefs.Current |> prefs.Set),
                                Never
                            )
                            CheckBox.onUnchecked (
                                (fun _ -> Optic.set PrefsOptic.setWithFoils false prefs.Current |> prefs.Set),
                                Never
                            )
                        ]
                    ]
                ]
            )
        ]
        :> IView

    let view prefs =
        Component.create (
            "preference-view",
            fun (ctx) ->
                let prefs = ctx.usePassed prefs

                render prefs
        )
