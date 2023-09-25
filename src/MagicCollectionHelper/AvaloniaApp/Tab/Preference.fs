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
    module View =
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
                                    label 2 "Number base"
                                    ComboBox.create [
                                        ComboBox.column 1
                                        ComboBox.row 2
                                        ComboBox.dataItems [ Decimal; Dozenal; Seximal ]
                                        ComboBox.selectedItem prefs.Current.numBase
                                        ComboBox.itemTemplate (
                                            DataTemplateView<NumBase>.create
                                                (fun numBase ->
                                                    TextBlock.create [ TextBlock.text (NumBase.toString numBase) ])
                                        )
                                        ComboBox.onSelectedItemChanged (fun v ->
                                            if v <> null then
                                                Optic.set PrefsOptic.numBase (unbox<NumBase> v) prefs.Current
                                                |> prefs.Set)
                                    ]
                                ]
                            ]
                        ]
                    ]
                )
            ]
            :> IView

    let create prefs =
        Component.create (
            "preference-view",
            fun (ctx) ->
                let prefs = ctx.usePassed prefs

                View.render prefs
        )
