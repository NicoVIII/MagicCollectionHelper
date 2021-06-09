module MagicCollectionHelper.AvaloniaApp.Components.Inventory.Update

open Elmish

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types
open MagicCollectionHelper.Core.Types.Generated

open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.Generated
open MagicCollectionHelper.AvaloniaApp.ViewHelper

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
        // TODO: move to prefs
        let minSize = 20
        let maxSize = 40

        /// This function traverses through the tree and collapses leafes which are too small
        let rec shrinkTreeHelper tree =
            let rec singleRun tree =
                match tree with
                | Nodes nodes ->
                    nodes
                    |> List.fold
                        (fun nodes' node ->
                            let names, child = node

                            match nodes', child with
                            | (lastNames, Leaf lastEntries) :: tail, Leaf entries ->
                                let lastAmount = List.length lastEntries
                                let amount = List.length entries

                                if (lastAmount + amount <= maxSize)
                                   || lastAmount < minSize then
                                    // We merge the leafs
                                    (List.append lastNames names, Leaf(List.append lastEntries entries))
                                    :: tail
                                else
                                    node :: nodes'
                            // We can eliminate Nodes with only one node
                            | _, Nodes [ childNode ] -> childNode :: nodes'
                            | _, (Nodes _ as tree) -> (names, singleRun tree) :: nodes'
                            | _, Leaf _ -> node :: nodes')
                        []
                    |> List.rev
                    |> Nodes
                | Leaf _ as leaf -> leaf

            // We repeat shrinking as long as there is something to shrink
            let shrunkTree = singleRun tree

            if shrunkTree <> tree then
                shrinkTreeHelper shrunkTree
            else
                shrunkTree

        let shrinkTree tree =
            tree
            |> HungTree.mapKey List.singleton
            |> shrinkTreeHelper
            |> HungTree.mapKey
                (function
                | [] -> failwith "Namelist was empty!"
                | [ name ] -> name
                // List is in reverse order, we merge the names
                | fName :: _ as nameList -> $"{fName} - {List.last nameList}")

        // Sort and group inventory
        let grouped =
            inventory
            |> List.map
                (fun (location, entries) ->
                    let cards =
                        entries
                        |> List.choose
                            (fun entry ->
                                Map.tryFind (entry.card.set, entry.card.number) infoMap
                                |> Option.map (CardEntryWithInfo.create entry))

                    (location, cards))
            |> List.choose
                (fun (location, entries) ->
                    let groupBy sortBy entries =
                        entries
                        |> List.groupBy
                            (fun entry ->
                                match sortBy with
                                | ByCmc -> $"CmC {entry.info.cmc}"
                                | BySet -> $"Set {entry.info.set.Value}"
                                | ByColorIdentity -> ColorIdentity.toString entry.info.colorIdentity
                                | ByName -> entry.info.name.Substring(0, 1)
                                | ByCollectorNumber -> entry.info.collectorNumber.Value
                                | ByLanguage langList ->
                                    let lang = entry.entry.card.language

                                    if List.contains lang langList then
                                        lang.Value
                                    else
                                        "Other"
                                | ByRarity rarities ->
                                    List.tryFind (Set.contains entry.info.rarity) rarities
                                    |> function
                                    | Some set ->
                                        set
                                        |> Set.map Rarity.toString
                                        |> Set.toSeq
                                        |> String.concat " / "
                                    | None -> "Other"
                                | ByTypeContains types ->
                                    List.tryFind (fun (typ: string) -> entry.info.typeLine.Contains typ) types
                                    |> Option.defaultValue "Other")

                    let rec createTree sortBy entries =
                        // Check conditions for the recursive call
                        let createTree lastSortBy sortBy entries =
                            // Some groupings are only using part of the sorting information
                            // therefore after them you can't group further without destroying order
                            match lastSortBy with
                            | ByName -> Leaf entries
                            | _ -> createTree sortBy entries

                        match sortBy, entries with
                        // The list is so small that further grouping has no benefit
                        | _, entries when List.length entries <= maxSize -> Leaf entries
                        // We have nothing more to group by
                        | [], _ -> Leaf entries
                        | sortBy :: tail, entries ->
                            // We try to group some more
                            let groups = groupBy sortBy entries

                            match groups with
                            // With just one group, grouping makes no sense
                            | [ (_, entries) ] -> createTree sortBy tail entries
                            | groups ->
                                groups
                                |> List.map (Tuple2.mapSnd (createTree sortBy tail))
                                |> Nodes

                    let sortBy =
                        match location with
                        | Fallback -> []
                        | Custom location -> location.sortBy

                    (location, createTree sortBy entries) |> Some)
            |> List.map (Tuple2.mapSnd shrinkTree)

        let viewMode =
            match state.viewMode with
            | Location location -> location
            | _ -> None
            |> Location

        let state =
            state
            |> setl StateLenses.filteredInventory grouped
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
            // TODO: Update rules of location
            //|> Map.change locationName (Option.map mutateRules)
            |> setlr StateLenses.locations state

        (state, Cmd.none)
