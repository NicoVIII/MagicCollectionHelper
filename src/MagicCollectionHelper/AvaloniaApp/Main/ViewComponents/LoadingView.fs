namespace MagicCollectionHelper.AvaloniaApp.Main.ViewComponents

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Main

module LoadingView =
    let loadingLine (icon: IView<Canvas>) text =
        StackPanel.create [
            StackPanel.orientation Orientation.Horizontal
            StackPanel.spacing 5.
            StackPanel.children [
                icon
                TextBlock.create [
                    TextBlock.fontSize 24.
                    TextBlock.text text
                ]
            ]
        ]

    let loadSetData state =
        let loadingState = getl StateLenses.setDataState state

        match loadingState with
        | Prepare -> Icons.sync, "Preparing set data import..."
        | Download -> Icons.sync, "Downloading set data..."
        | Import -> Icons.sync, "Importing set data..."
        | Ready -> Icons.check, "Set data ready!"
        ||> loadingLine

    let loadCardInfo state =
        let loadingState = getl StateLenses.cardInfoState state

        match loadingState with
        | Prepare -> Icons.sync, "Preparing card info import..."
        | Download -> Icons.sync, "Downloading card info..."
        | Import -> Icons.sync, "Importing card info..."
        | Ready -> Icons.check, "Card info ready!"
        ||> loadingLine

    let loadCollection state =
        let loadingState = getl StateLenses.dsEntriesState state

        match loadingState with
        | Prepare -> Icons.sync, "Preparing collection import..."
        | Import -> Icons.sync, "Importing collection..."
        | Ready -> Icons.check, "Collection imported!"
        | Download -> failwith "Invalid state"
        ||> loadingLine

    let processEntries state =
        let loadingState = getl StateLenses.entriesState state

        match loadingState with
        | Prepare -> Icons.reloadAlert, "Waiting..."
        | Import -> Icons.reload, "Process collection..."
        | Ready -> Icons.check, "Collection ready!"
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
                loadCardInfo state
                loadCollection state
                processEntries state
            ]
        ]
        :> IView
