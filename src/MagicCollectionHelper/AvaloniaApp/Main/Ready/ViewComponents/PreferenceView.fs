namespace MagicCollectionHelper.AvaloniaApp.Main.Ready.ViewComponents

open Avalonia.Controls
open Avalonia.Layout

open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp.Main.Ready
open MagicCollectionHelper.AvaloniaApp.ViewHelper

module PreferenceView =
    let numInputProps dispatch (min, max) lens prefs =
        let currentValue = getl lens prefs |> double

        [ NumericUpDown.margin (10., 0., 0., 0.)
          NumericUpDown.maximum max
          NumericUpDown.minimum min
          NumericUpDown.value currentValue
          NumericUpDown.onValueChanged (
              (fun newValue ->
                  if currentValue <> newValue then
                      newValue |> setl lens |> ChangePrefs |> dispatch),
              OnChangeOf currentValue
          ) ]

    let withTableProps row column attrList =
        [ NumericUpDown.column column
          NumericUpDown.row row ]
        |> List.append attrList

    let render (state: State) (dispatch: Dispatch) : IView =
        let numInputProps = numInputProps dispatch

        let prefs = getl StateLenses.prefs state

        // Lens to convert uintToDouble
        let uintDoubleLens = Lens(double, (fun _ v -> uint v))

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
                                        numInputProps
                                            (0., 200.)
                                            (PrefsLenses.cardGroupMinSize << uintDoubleLens)
                                            prefs
                                        |> NumericUpDown.create

                                        numInputProps
                                            (0., 200.)
                                            (PrefsLenses.cardGroupMaxSize << uintDoubleLens)
                                            prefs
                                        |> NumericUpDown.create
                                    ]
                                ]
                                label 2 "Missing from percent"
                                NumericUpDown.create [
                                    NumericUpDown.column 1
                                    NumericUpDown.margin (10., 0., 0., 0.)
                                    NumericUpDown.maximum 100.
                                    NumericUpDown.minimum 0.
                                    NumericUpDown.row 2
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
                                label 4 "Number base"
                                ComboBox.create [
                                    ComboBox.column 1
                                    ComboBox.row 4
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
                                    ComboBox.onSelectedItemChanged (fun v ->
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
