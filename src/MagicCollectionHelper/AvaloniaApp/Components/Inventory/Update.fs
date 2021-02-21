module MagicCollectionHelper.AvaloniaApp.Components.Inventory.Update

open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components.Inventory

let perform (entries: CardEntry list) (msg: Msg) (state: State): State =
    match msg with
    | TakeInventory ->
        Inventory.take state.locations entries
        |> setlr StateLenses.inventory state
