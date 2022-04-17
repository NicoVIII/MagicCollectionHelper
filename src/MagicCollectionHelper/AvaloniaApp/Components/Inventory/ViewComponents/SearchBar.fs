namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory.ViewComponents

open Avalonia.Controls
open Avalonia.Layout

open Avalonia.FuncUI.DSL

open SimpleOptics

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.Generated

module SearchBar =
    let searchText search dispatch =
        TextBox.create [
            TextBox.text search.text
            TextBox.onTextChanged (
                (fun text ->
                    if search.text <> text then
                        text
                        |> Optic.set (Optic.compose StateLenses.search SearchLenses.text)
                        |> ChangeState
                        |> dispatch),
                OnChangeOf search.text
            )
        ]

    let searchOld search dispatch =
        let update value =
            (fun (_: 'a) ->
                if search.old <> value then
                    value
                    |> Optic.set (Optic.compose StateLenses.search SearchLenses.old)
                    |> ChangeState
                    |> dispatch)

        let subPatch = OnChangeOf search.old

        StackPanel.create [
            StackPanel.orientation Orientation.Horizontal
            StackPanel.children [
                CheckBox.create [
                    CheckBox.content "Only new"
                    CheckBox.isChecked (
                        match search.old with
                        | Some false -> true
                        | _ -> false
                    )
                    CheckBox.onChecked (update (Some false), subPatch)
                    CheckBox.onUnchecked (update None, subPatch)
                ]
            ]
        ]

    let render search dispatch =
        Border.create [
            Border.borderBrush Config.lineColor
            Border.borderThickness (0., 0., 0., 1.)
            Border.dock Dock.Top
            Border.padding 5.
            Border.child (
                StackPanel.create [
                    StackPanel.orientation Orientation.Vertical
                    StackPanel.spacing 4.
                    StackPanel.children [
                        searchText search dispatch
                        searchOld search dispatch
                    ]
                ]
            )
        ]
