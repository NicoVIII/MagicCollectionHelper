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
      locations: MagicCollectionHelper.Core.Types.CustomLocation list }

type Msg =
    | TakeInventory
    | SaveInventory of LocationCardMap
    | OpenLocationEdit
    | CloseLocationEdit

type Dispatch = Msg -> unit

module Model =
    open MagicCollectionHelper.Core
    open MagicCollectionHelper.Core.Types

    let init () : State =
        // Test Locations
        let locations =
            [ { name = "Collection GRN"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    [ InSet(
                        [ "GRN"; "TGRN" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      LimitExact 1u
                      IsFoil false ] }
              { name = "Collection RNA"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    [ InSet(
                        [ "RNA"; "TRNA" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      LimitExact 1u
                      IsFoil false ] }
              { name = "Collection WAR"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    [ InSet(
                        [ "WAR"; "TWAR" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      LimitExact 1u
                      IsFoil false ] }
              { name = "Collection ELD"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    [ InSet(
                        [ "ELD"; "TELD" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      LimitExact 1u
                      IsFoil false ] }
              { name = "Collection THB"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    [ InSet(
                        [ "THB"; "TTHB" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      LimitExact 1u
                      IsFoil false ] }
              { name = "Collection IKO"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    [ InSet(
                        [ "IKO"; "TIKO" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      LimitExact 1u
                      IsFoil false ] }
              { name = "Collection ZNR"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    [ InSet(
                        [ "ZNR"; "TZNR" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      LimitExact 1u
                      IsFoil false ] }
              { name = "Collection KHM"
                sortBy = [ BySet; ByCollectorNumber ]
                rules =
                    [ InSet(
                        [ "KHM"; "TKHM" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      LimitExact 1u
                      IsFoil false ] }
              { name = "Lookup (Colorless)"
                sortBy = [ ByName ]
                rules =
                    [ Limit 1u
                      ColorIdentity([ [] ] |> Set.ofListList) ] }
              { name = "Lookup (White)"
                sortBy = [ ByName ]
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ White ] ] |> Set.ofListList) ] }
              { name = "Lookup (Blue)"
                sortBy = [ ByName ]
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ Blue ] ] |> Set.ofListList) ] }
              { name = "Lookup (Black)"
                sortBy = [ ByName ]
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ Black ] ] |> Set.ofListList) ] }
              { name = "Lookup (Red)"
                sortBy = [ ByName ]
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ Red ] ] |> Set.ofListList) ] }
              { name = "Lookup (Green)"
                sortBy = [ ByName ]
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ Green ] ] |> Set.ofListList) ] }
              { name = "Lookup (Mixed 1)"
                sortBy = [ ByName ]
                rules =
                    [ Limit 1u
                      ColorIdentity(
                          [ [ White; Blue ]
                            [ Blue; Black ]
                            [ Black; Red ]
                            [ Red; Green ]
                            [ Green; White ] ]
                          |> Set.ofListList
                      ) ] }
              { name = "Lookup (Mixed 2)"
                sortBy = [ ByName ]
                rules = [ Limit 1u ] } ]

        { editLocations = false
          inventory = Map.empty
          loadInProgress = false
          locations = locations }
