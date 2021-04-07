namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory

open Myriad.Plugins
open System

type LocationCardMap =
    Map<MagicCollectionHelper.Core.Types.InventoryLocation, MagicCollectionHelper.Core.Types.CardEntry list>

[<Generator.Lenses("components-inventory", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { editLocations: bool
      inventory: LocationCardMap
      loadInProgress: bool
      locations: Map<MagicCollectionHelper.Core.Types.CustomLocationName, MagicCollectionHelper.Core.Types.CustomLocation> }

open MagicCollectionHelper.Core.Types

type Msg =
    | TakeInventory
    | SaveInventory of LocationCardMap
    | OpenLocationEdit
    | CloseLocationEdit
    | UpdateLocationRules of CustomLocationName * (Rules -> Rules)

type Dispatch = Msg -> unit

module Model =
    open MagicCollectionHelper.Core

    let init () : State =
        // Test Locations
        let locations =
            [ { name = "Collection GRN"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    Rules.empty
                    |> Rules.withInSet (
                        [ "GRN"; "TGRN" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                    )
                    |> Rules.withLimitExact 1u
                    |> Rules.withIsFoil false }
              { name = "Collection RNA"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    Rules.empty
                    |> Rules.withInSet (
                        [ "RNA"; "TRNA" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                    )
                    |> Rules.withLimitExact 1u
                    |> Rules.withIsFoil false }
              { name = "Collection WAR"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    Rules.empty
                    |> Rules.withInSet (
                        [ "WAR"; "TWAR" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                    )
                    |> Rules.withLimitExact 1u
                    |> Rules.withIsFoil false }
              { name = "Collection ELD"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    Rules.empty
                    |> Rules.withInSet (
                        [ "ELD"; "TELD" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                    )
                    |> Rules.withLimitExact 1u
                    |> Rules.withIsFoil false }
              { name = "Collection THB"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    Rules.empty
                    |> Rules.withInSet (
                        [ "THB"; "TTHB" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                    )
                    |> Rules.withLimitExact 1u
                    |> Rules.withIsFoil false }
              { name = "Collection IKO"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    Rules.empty
                    |> Rules.withInSet (
                        [ "IKO"; "TIKO" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                    )
                    |> Rules.withLimitExact 1u
                    |> Rules.withIsFoil false }
              { name = "Collection ZNR"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    Rules.empty
                    |> Rules.withInSet (
                        [ "ZNR"; "TZNR" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                    )
                    |> Rules.withLimitExact 1u
                    |> Rules.withIsFoil false }
              { name = "Collection KHM"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    Rules.empty
                    |> Rules.withInSet (
                        [ "KHM"; "TKHM" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                    )
                    |> Rules.withLimitExact 1u
                    |> Rules.withIsFoil false }
              { name = "Lookup 1 (Colorless)"
                sortBy = [ ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [] ] |> Set.ofListList)
                    |> Rules.withRarity ([ Uncommon; Rare; Mythic ] |> Set.ofList) }
              { name = "Lookup 2 (Colorless)"
                sortBy = [ ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [] ] |> Set.ofListList)
                    |> Rules.withRarity ([ Common ] |> Set.ofList) }
              { name = "Lookup 1 (White)"
                sortBy = [ ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ White ] ] |> Set.ofListList)
                    |> Rules.withRarity ([ Uncommon; Rare; Mythic ] |> Set.ofList) }
              { name = "Lookup 2 (White)"
                sortBy = [ ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ White ] ] |> Set.ofListList)
                    |> Rules.withRarity ([ Common ] |> Set.ofList) }
              { name = "Lookup (Blue)"
                sortBy = [ ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ Blue ] ] |> Set.ofListList) }
              { name = "Lookup (Black)"
                sortBy = [ ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ Black ] ] |> Set.ofListList) }
              { name = "Lookup (Red)"
                sortBy = [ ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ Red ] ] |> Set.ofListList) }
              { name = "Lookup (Green)"
                sortBy = [ ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ Green ] ] |> Set.ofListList) }
              { name = "Lookup (Mixed 1)"
                sortBy = [ ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity (
                        [ [ White; Blue ]
                          [ Blue; Black ]
                          [ Black; Red ]
                          [ Red; Green ]
                          [ Green; White ] ]
                        |> Set.ofListList
                    ) }
              { name = "Lookup (Mixed 2)"
                sortBy = [ ByName ]
                rules = Rules.empty |> Rules.withLimit 1u } ]
            // Pack position into location
            |> List.indexed
            |> List.map (fun (pos, location) -> CustomLocation.createFromRaw (uint pos) location)
            // Extract name and provide as map key
            |> List.map (fun location -> (location.name, location))
            |> Map.ofList

        { editLocations = false
          inventory = Map.empty
          loadInProgress = false
          locations = locations }
