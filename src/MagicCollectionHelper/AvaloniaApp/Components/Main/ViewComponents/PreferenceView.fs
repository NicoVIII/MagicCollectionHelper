namespace MagicCollectionHelper.AvaloniaApp.Components.Main.ViewComponents

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout

open MagicCollectionHelper.AvaloniaApp.Components.Main
open MagicCollectionHelper.AvaloniaApp.ViewHelper
open MagicCollectionHelper.Core.Types

module PreferenceView =
    let render (state: State) (dispatch: Dispatch): IView =
        let prefs = state.prefs

        Border.create [
            Border.padding 10.
            Border.child (
                StackPanel.create [
                    StackPanel.spacing 5.
                    StackPanel.children [
                        Grid.create [
                            Grid.columnDefinitions "Auto, Auto"
                            Grid.children [
                                label 0 "Missing from percent"
                                NumericUpDown.create [
                                    NumericUpDown.column 1
                                    NumericUpDown.margin (10.,0.,0.,0.)
                                    NumericUpDown.maximum 100.
                                    NumericUpDown.minimum 0.
                                    NumericUpDown.row 0
                                    NumericUpDown.value (prefs.missingPercent * 100.)
                                    NumericUpDown.onValueChanged ((fun value ->
                                        value / 100.
                                        |> setlr PrefsLenses.missingPercent prefs
                                        |> ChangePrefs
                                        |> dispatch
                                    ), Always)
                                ]
                            ]
                        ]
                        CheckBox.create [
                            CheckBox.content "Include Foils in Set Analyser"
                            CheckBox.isChecked prefs.setWithFoils
                            CheckBox.onChecked ((fun _ ->
                                setl PrefsLenses.setWithFoils true prefs |> ChangePrefs |> dispatch), Always)
                            CheckBox.onUnchecked ((fun _ ->
                                setl PrefsLenses.setWithFoils false prefs |> ChangePrefs |> dispatch), Always)
                        ]
                        CheckBox.create [
                            CheckBox.content "Dozenalize"
                            CheckBox.isChecked prefs.dozenalize
                            CheckBox.onChecked ((fun _ ->
                                setl PrefsLenses.dozenalize true prefs |> ChangePrefs |> dispatch), Always)
                            CheckBox.onUnchecked ((fun _ ->
                                setl PrefsLenses.dozenalize false prefs |> ChangePrefs |> dispatch), Always)
                        ]
                    ]
                ]
            )
        ]:> IView