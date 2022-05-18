module MagicCollectionHelper.AvaloniaApp.Components.Inventory.Update

open Elmish
open SimpleOptics

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.Generated
open MagicCollectionHelper.AvaloniaApp.ViewHelper

let perform
    (prefs: Prefs)
    (setData: SetDataMap)
    (infoMap: CardInfoMap)
    (entries: OldAmountable<Entry> list)
    (msg: Msg)
    state
    =
    match msg with
    | AsyncError error -> raise error
    | ChangeState map -> map state, Cmd.none
    | TakeInventory ->
        let state = state |> (Optic.set StateLenses.viewMode Loading)

        let fnc () =
            Inventory.takeAsync setData infoMap state.locations entries

        let cmd = Cmd.OfAsync.perform fnc () SaveInventory

        state, cmd
    | FilterInventory inventory ->
        let minSize = prefs.cardGroupMinSize |> int
        let maxSize = prefs.cardGroupMaxSize |> int

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
            |> HungTree.mapKey (List.singleton)
            |> shrinkTreeHelper
            |> HungTree.mapKey (function
                | [] -> failwith "Namelist was empty!"
                | [ name ] -> name
                // List is in reverse order, we merge the names
                | fName :: _ as nameList -> $"{fName} - {List.last nameList}")

        // Sort and group inventory
        let grouped =
            inventory
            |> List.map (fun (location, entries) ->
                let cards =
                    entries
                    |> List.choose (fun agedEntry ->
                        let entry = agedEntry.data

                        Map.tryFind (entry.card.set, entry.card.number) infoMap
                        |> Option.map (fun cardInfo -> AgedEntryWithInfo.create cardInfo agedEntry))

                (location, cards))
            |> List.choose (fun (location, entries) ->
                let groupBy nameOffset sortBy entries =
                    entries
                    |> List.groupBy (fun (entry: AgedEntryWithInfo) ->
                        if nameOffset > 0 then
                            (entry ^. AgedEntryWithInfoLenses.name)
                                .Substring(0, nameOffset + 1)
                        else
                            match sortBy with
                            | ByCmc ->
                                entry ^. AgedEntryWithInfoLenses.cmc
                                |> sprintf "CmC %i"
                            | BySet ->
                                entry ^. AgedEntryWithInfoLenses.set
                                |> MagicSet.unwrap
                                |> sprintf "Set %s"
                            | ByColorIdentity ->
                                entry ^. AgedEntryWithInfoLenses.colorIdentity
                                |> ColorIdentity.toString
                            | ByName ->
                                (entry ^. AgedEntryWithInfoLenses.name)
                                    .Substring(0, 1)
                            | ByCollectorNumber ->
                                entry ^. AgedEntryWithInfoLenses.number
                                |> CollectorNumber.unwrap
                            | ByLanguage langList ->
                                let language = entry ^. AgedEntryWithInfoLenses.language

                                if List.contains language langList then
                                    Language.unwrap language
                                else
                                    "Other"
                            | ByRarity rarities ->
                                let rarity = entry ^. AgedEntryWithInfoLenses.rarity

                                List.tryFind (Set.contains rarity) rarities
                                |> function
                                    | Some set ->
                                        set
                                        |> Set.map Rarity.toString
                                        |> Set.toSeq
                                        |> String.concat " / "
                                    | None -> "Other"
                            | ByTypeContains types ->
                                let typeLine = entry ^. AgedEntryWithInfoLenses.typeLine

                                List.tryFind (fun (typ: string) -> typeLine.Contains typ) types
                                |> Option.defaultValue "Other")

                let rec createTree nameOffset sortBy entries =
                    // Check conditions for the recursive call
                    let createTree lastSortBy sortBy entries =
                        // Some groupings are only using part of the sorting information
                        // therefore after them you can't group further without destroying order
                        match lastSortBy with
                        | ByName -> createTree (nameOffset + 1) sortBy entries
                        | _ -> createTree nameOffset sortBy entries

                    match sortBy, entries with
                    // The list is so small that further grouping has no benefit
                    | _, entries when List.length entries <= maxSize -> Leaf entries
                    // We have nothing more to group by
                    | [], _ -> Leaf entries
                    | sortBy :: tail, entries ->
                        // We try to group some more
                        let groups = groupBy nameOffset sortBy entries

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

                (location, createTree 0 sortBy entries) |> Some)
            // We shrink the tree and merge small groups of cards
            |> List.map (Tuple2.mapSnd (shrinkTree))

        let viewMode =
            match state.viewMode with
            | Location location -> location
            | _ -> None
            |> Location

        let state =
            state
            |> (Optic.set StateLenses.filteredInventory grouped)
            |> (Optic.set StateLenses.viewMode viewMode)

        (state, Cmd.none)
    | SaveInventory inventory ->
        let state = state |> Optic.set StateLenses.inventory inventory

        (state, Cmd.ofMsg (FilterInventory inventory))
    | ChangeLocation location ->
        let state =
            state
            |> Optic.set StateLenses.viewMode (Some location |> Location)

        state, Cmd.none
    | OpenLocationEdit ->
        let state = state |> Optic.set StateLenses.viewMode Edit

        (state, Cmd.none)
    | CloseLocationEdit ->
        let state =
            state
            |> Optic.set StateLenses.viewMode (Location None)

        (state, Cmd.none)
    | UpdateLocationRules (locationName, rulesMutation) ->
        let mutateRules location =
            Optic.map CustomLocationLenses.rules rulesMutation location

        let state =
            state
            // TODO: Update rules of location
            // (Map.change locationName (Option.map mutateRules))
            |> Optic.map StateLenses.locations id

        (state, Cmd.none)
