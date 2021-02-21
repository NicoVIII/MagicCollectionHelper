module MagicCollectionHelper.AvaloniaApp.Components.Inventory.Update

open Elmish
open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components.Inventory

let perform (entries: CardEntry list) (msg: Msg) (state: State) =
    match msg with
    | TakeInventory ->
        let state = setl StateLenses.loadInProgress true state

        let fnc () = Inventory.takeAsync state.locations entries
        let cmd = Cmd.OfAsync.perform fnc () SaveInventory

        (state, cmd)
    | SaveInventory inventory ->
        let state =
            inventory
            |> setlr StateLenses.inventory state
            |> setl StateLenses.loadInProgress false
        (state, Cmd.none)
