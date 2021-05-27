namespace MagicCollectionHelper.AvaloniaApp.Main.ViewComponents

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Main

module LoadingView =
    let loadingTextBlock text =
        TextBlock.create [
            TextBlock.fontSize 24.
            TextBlock.text text
        ]

    let loadSetData state =
        let loadingState = getl StateLenses.setDataState state

        match loadingState with
        | Prepare -> "Preparing set data import..."
        | Download -> "Downloading set data..."
        | Import -> "Importing set data..."
        | Ready -> "Set data ready!"
        |> loadingTextBlock

    let loadCardInfo state =
        let loadingState = getl StateLenses.cardInfoState state

        match loadingState with
        | Prepare -> "Preparing card info import..."
        | Download -> "Downloading card info..."
        | Import -> "Importing card info..."
        | Ready -> "Card info ready!"
        |> loadingTextBlock

    let render (state: State) (dispatch: Dispatch) : IView =
        StackPanel.create [
            StackPanel.horizontalAlignment HorizontalAlignment.Center
            StackPanel.verticalAlignment VerticalAlignment.Center
            StackPanel.spacing 25.
            StackPanel.children [
                loadSetData state
                loadCardInfo state
            ]
        ]
        :> IView
