namespace MagicCollectionHelper.AvaloniaApp.Main.Ready.ViewComponents

open Avalonia.Controls

open Avalonia.FuncUI.Components
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp.Main.Ready
open MagicCollectionHelper.AvaloniaApp.ViewHelper

module PreferenceView =
    let render (state: State) (dispatch: Dispatch) : IView =
        let prefs = getl StateLenses.prefs state

        Border.create [
            Border.padding 10.
            Border.child (
                StackPanel.create [
                    StackPanel.spacing 5.
                    StackPanel.children [
                        Grid.create [
                            Grid.columnDefinitions "Auto, Auto"
                            Grid.rowDefinitions (List.replicate 2 "Auto" |> String.concat ",5,")
                            Grid.children [
                                label 0 "Missing from percent"
                                NumericUpDown.create [
                                    NumericUpDown.column 1
                                    NumericUpDown.margin (10., 0., 0., 0.)
                                    NumericUpDown.maximum 100.
                                    NumericUpDown.minimum 0.
                                    NumericUpDown.row 0
                                    NumericUpDown.value (prefs.missingPercent * 100.)
                                    NumericUpDown.onValueChanged (
                                        (fun value ->
                                            if value <> prefs.missingPercent * 100. then
                                                value / 100.
                                                |> setl PrefsLenses.missingPercent
                                                |> ChangePrefs
                                                |> dispatch),
                                        OnChangeOf prefs.missingPercent
                                    )
                                ]
                                label 2 "Number base"
                                ComboBox.create [
                                    ComboBox.column 1
                                    ComboBox.row 2
                                    ComboBox.dataItems [
                                        Decimal
                                        Dozenal
                                        Seximal
                                    ]
                                    ComboBox.selectedItem prefs.numBase
                                    ComboBox.itemTemplate (
                                        DataTemplateView<NumBase>.create
                                            (fun numBase ->
                                                TextBlock.create [
                                                    TextBlock.text (NumBase.toString numBase)
                                                ])
                                    )
                                    ComboBox.onSelectedItemChanged
                                        (fun v ->
                                            if v <> null then
                                                v
                                                |> unbox<NumBase>
                                                |> setl PrefsLenses.numBase
                                                |> ChangePrefs
                                                |> dispatch)
                                ]
                            ]
                        ]
                        CheckBox.create [
                            CheckBox.content "Include Foils in Set Analyser"
                            CheckBox.isChecked prefs.setWithFoils
                            CheckBox.onChecked (
                                (fun _ ->
                                    (PrefsLenses.setWithFoils .-> true)
                                    |> ChangePrefs
                                    |> dispatch),
                                Never
                            )
                            CheckBox.onUnchecked (
                                (fun _ ->
                                    (PrefsLenses.setWithFoils .-> false)
                                    |> ChangePrefs
                                    |> dispatch),
                                Never
                            )
                        ]
                    ]
                ]
            )
        ]
        :> IView
