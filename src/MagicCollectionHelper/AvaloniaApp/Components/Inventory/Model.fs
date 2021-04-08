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

    let typeSortDefault =
        [ "Land"
          "Creature"
          "Sorcery"
          "Instant"
          "Enchantment"
          "Artifact"
          "Planeswalker" ]
        |> ByTypeContains

    let raritySortDefault =
        [ [ Common ]
          [ Uncommon ]
          [ Rare; Mythic ] ]
        |> List.map Set.ofList
        |> ByRarity

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
                    |> Rules.withIsFoil false
                    |> Rules.withInLanguage (Language "en") }
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
                    |> Rules.withIsFoil false
                    |> Rules.withInLanguage (Language "en") }
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
                    |> Rules.withIsFoil false
                    |> Rules.withInLanguage (Language "en") }
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
                    |> Rules.withIsFoil false
                    |> Rules.withInLanguage (Language "en") }
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
                    |> Rules.withIsFoil false
                    |> Rules.withInLanguage (Language "en") }
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
                    |> Rules.withIsFoil false
                    |> Rules.withInLanguage (Language "en") }
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
                    |> Rules.withIsFoil false
                    |> Rules.withInLanguage (Language "en") }
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
                    |> Rules.withIsFoil false
                    |> Rules.withInLanguage (Language "en") }
              { name = "Foil basic lands"
                sortBy =
                    [ ByColorIdentity
                      BySet
                      ByCollectorNumber ]
                rules =
                    Rules.empty
                    |> Rules.withLimitExact 1u
                    |> Rules.withIsFoil true
                    |> Rules.withTypeContains ([ "Basic Land" ] |> Set.ofList) }
              { name = "Lookup 1 (Lands)"
                sortBy = [ ByColorIdentity; BySet ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withTypeContains ([ "Land" ] |> Set.ofList)
                    |> Rules.withTypeNotContains ([ "Basic Land" ] |> Set.ofList)
                    |> Rules.withRarity ([ Rare; Mythic; Special; Bonus ] |> Set.ofList) }
              { name = "Lookup 1 (Colorless)"
                sortBy = [ typeSortDefault; ByCmc; BySet ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [] ] |> Set.ofListList)
                    |> Rules.withRarity ([ Rare; Mythic; Special; Bonus ] |> Set.ofList)
                    |> Rules.withTypeNotContains ([ "Land" ] |> Set.ofList)
                    |> Rules.withIsToken false }
              { name = "Lookup 2 (Lands)"
                sortBy = [ ByColorIdentity; ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withTypeContains ([ "Land" ] |> Set.ofList)
                    |> Rules.withTypeNotContains ([ "Basic Land" ] |> Set.ofList)
                    |> Rules.withRarity ([ Common; Uncommon ] |> Set.ofList) }
              { name = "Lookup 2 (Colorless)"
                sortBy = [ typeSortDefault; ByCmc; ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [] ] |> Set.ofListList)
                    |> Rules.withRarity ([ Common; Uncommon ] |> Set.ofList)
                    |> Rules.withTypeNotContains ([ "Land" ] |> Set.ofList)
                    |> Rules.withIsToken false }
              { name = "Lookup 1 (White)"
                sortBy =
                    [ typeSortDefault
                      ByCmc
                      raritySortDefault
                      BySet ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ White ] ] |> Set.ofListList)
                    |> Rules.withRarity (
                        [ Uncommon
                          Rare
                          Mythic
                          Special
                          Bonus ]
                        |> Set.ofList
                    )
                    |> Rules.withTypeNotContains ([ "Land" ] |> Set.ofList)
                    |> Rules.withIsToken false }
              { name = "Lookup 2 (White)"
                sortBy = [ typeSortDefault; ByCmc; ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ White ] ] |> Set.ofListList)
                    |> Rules.withRarity ([ Common ] |> Set.ofList)
                    |> Rules.withTypeNotContains ([ "Land" ] |> Set.ofList)
                    |> Rules.withIsToken false }
              { name = "Lookup 1 (Blue)"
                sortBy =
                    [ typeSortDefault
                      ByCmc
                      raritySortDefault
                      BySet ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ Blue ] ] |> Set.ofListList)
                    |> Rules.withRarity (
                        [ Uncommon
                          Rare
                          Mythic
                          Special
                          Bonus ]
                        |> Set.ofList
                    )
                    |> Rules.withTypeNotContains ([ "Land" ] |> Set.ofList)
                    |> Rules.withIsToken false }
              { name = "Lookup 2 (Blue)"
                sortBy = [ typeSortDefault; ByCmc; ByName ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ Blue ] ] |> Set.ofListList)
                    |> Rules.withRarity ([ Common ] |> Set.ofList)
                    |> Rules.withTypeNotContains ([ "Land" ] |> Set.ofList)
                    |> Rules.withIsToken false }
              { name = "Lookup (Black)"
                sortBy = [ typeSortDefault; ByCmc ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ Black ] ] |> Set.ofListList) }
              { name = "Lookup (Red)"
                sortBy = [ typeSortDefault; ByCmc ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ Red ] ] |> Set.ofListList) }
              { name = "Lookup (Green)"
                sortBy = [ typeSortDefault; ByCmc ]
                rules =
                    Rules.empty
                    |> Rules.withLimit 1u
                    |> Rules.withColorIdentity ([ [ Green ] ] |> Set.ofListList) }
              { name = "Lookup (Mixed 1)"
                sortBy = [ typeSortDefault; ByCmc ]
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
                sortBy = [ ByCmc ]
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
