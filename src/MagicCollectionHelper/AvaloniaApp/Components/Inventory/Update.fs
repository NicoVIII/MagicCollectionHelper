module MagicCollectionHelper.AvaloniaApp.Components.Inventory.Update

open Elmish
open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components.Inventory

let perform (infoMap: CardInfoMap) (entries: CardEntry list) (msg: Msg) (state: State) =
    match msg with
    | TakeInventory ->
        let state =
            setl StateLenses.loadInProgress true state

        let fnc () =
            Inventory.takeAsync infoMap state.locations entries

        let cmd = Cmd.OfAsync.perform fnc () SaveInventory

        (state, cmd)
    | SaveInventory inventory ->
        let state =
            inventory
            |> setlr StateLenses.inventory state
            |> setl StateLenses.loadInProgress false

        (state, Cmd.none)
    | OpenLocationEdit ->
        let state =
            setl StateLenses.editLocations true state

        (state, Cmd.none)
    | CloseLocationEdit ->
        let state =
            setl StateLenses.editLocations false state

        (state, Cmd.none)
    | UpdateLocationRules (locationName, rules) ->
        let state =
            state
            |> getl StateLenses.locations
            // Update rules of location
            |> Map.change locationName (Option.map (setl CustomLocationLenses.rules rules))
            |> setlr StateLenses.locations state

        (state, Cmd.none)
