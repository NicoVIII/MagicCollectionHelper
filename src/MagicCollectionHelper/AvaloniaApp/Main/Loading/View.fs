namespace MagicCollectionHelper.AvaloniaApp.Main.Loading

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout

open MagicCollectionHelper.AvaloniaApp

module View =
    let loadingLine (icon: IView<Canvas>) text =
        StackPanel.create [
            StackPanel.orientation Orientation.Horizontal
            StackPanel.spacing 5.
            StackPanel.children [ icon; TextBlock.create [ TextBlock.fontSize 24.; TextBlock.text text ] ]
        ]

    let loadSetData state =
        match state.setData with
        | Prepare -> Icons.sync, "Preparing set data import..."
        | Download -> Icons.sync, "Downloading set data..."
        | Import -> Icons.sync, "Importing set data..."
        | Ready _ -> Icons.check, "Set data ready!"
        ||> loadingLine

    let loadCardInfo state =
        match state.cardInfo with
        | Prepare -> Icons.sync, "Preparing card info import..."
        | Download -> Icons.sync, "Downloading card info..."
        | Import -> Icons.sync, "Importing card info..."
        | Ready _ -> Icons.check, "Card info ready!"
        ||> loadingLine

    let loadCollection state =
        match state.dsEntries with
        | Prepare -> Icons.sync, "Preparing collection import..."
        | Import -> Icons.sync, "Importing collection..."
        | Ready _ -> Icons.check, "Collection imported!"
        | Download -> failwith "Invalid state"
        ||> loadingLine

    let processEntries state =
        match state.entries with
        | Prepare -> Icons.reloadAlert, "Waiting..."
        | Import -> Icons.reload, "Process collection..."
        | Ready _ -> Icons.check, "Collection ready!"
        | Download -> failwith "Invalid state"
        ||> loadingLine

    let render (state: State) (dispatch: Dispatch) : IView =
        StackPanel.create [
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.minWidth 280.
            StackPanel.spacing 25.
            StackPanel.verticalAlignment VerticalAlignment.Center
            StackPanel.children [
                loadSetData state
                loadCollection state
                loadCardInfo state
                processEntries state
            ]
        ]
        :> IView
