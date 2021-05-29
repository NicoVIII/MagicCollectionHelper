module MagicCollectionHelper.AvaloniaApp.Components.Inventory.Update

open Elmish
open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.Generated

let perform (setData: SetDataMap) (infoMap: CardInfoMap) (entries: CardEntry list) (msg: Msg) (state: State) =
    match msg with
    | TakeInventory ->
        let state =
            setl StateLenses.loadInProgress true state

        let fnc () =
            Inventory.takeAsync setData infoMap state.locations entries

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
    | UpdateLocationRules (locationName, rulesMutation) ->
        let mutateRules location =
            getl CustomLocationLenses.rules location
            |> rulesMutation
            |> setlr CustomLocationLenses.rules location

        let state =
            state
            |> getl StateLenses.locations
            // Update rules of location
            |> Map.change locationName (Option.map mutateRules)
            |> setlr StateLenses.locations state

        (state, Cmd.none)
