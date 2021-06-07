module MagicCollectionHelper.AvaloniaApp.Components.Inventory.Update

open Elmish

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.Generated

let perform (setData: SetDataMap) (infoMap: CardInfoMap) (entries: CardEntry list) (msg: Msg) (state: State) =
    match msg with
    | AsyncError error -> raise error
    | TakeInventory ->
        let state =
            state |> setl StateLenses.viewMode Loading

        let fnc () =
            Inventory.takeAsync setData infoMap state.locations entries

        let cmd = Cmd.OfAsync.perform fnc () SaveInventory

        (state, cmd)
    | FilterInventory inventory ->
        // Sort inventory
        let filtered =
            inventory
            |> Map.toList
            |> List.sortBy
                (fun (location, _) ->
                    // We sort like our locations are sorted
                    Map.tryPick
                        (fun _ l ->
                            if Custom l = location then
                                Some l.position
                            else
                                None)
                        state.locations
                    |> Option.defaultValue 999u)
            |> List.map
                (fun (location, entries) ->
                    let cards =
                        entries
                        |> List.choose
                            (fun entry ->
                                Map.tryFind (entry.card.set, entry.card.number) infoMap
                                |> Option.map (CardEntryWithInfo.create entry))

                    (location, cards))

        let viewMode =
            match state.viewMode with
            | Location location -> location
            | _ -> None
            |> Location

        let state =
            state
            |> setl StateLenses.filteredInventory filtered
            |> setl StateLenses.viewMode viewMode

        (state, Cmd.none)
    | SaveInventory inventory ->
        let state =
            state |> setl StateLenses.inventory inventory

        (state, Cmd.ofMsg (FilterInventory inventory))
    | ChangeSearchString term ->
        let state = state |> setl StateLenses.search term

        state, Cmd.none
    | ChangeLocation location ->
        let state =
            state
            |> setl StateLenses.viewMode (Some location |> Location)

        state, Cmd.none
    | OpenLocationEdit ->
        let state = state |> setl StateLenses.viewMode Edit

        (state, Cmd.none)
    | CloseLocationEdit ->
        let state =
            state |> setl StateLenses.viewMode (Location None)

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
