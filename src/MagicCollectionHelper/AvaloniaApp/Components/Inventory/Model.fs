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
                rules =
                    [ InSet(
                        [ "GRN"; "TGRN" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      Limit 1u
                      IsFoil false ] }
              { name = "Collection RNA"
                rules =
                    [ InSet(
                        [ "RNA"; "TRNA" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      Limit 1u
                      IsFoil false ] }
              { name = "Collection WAR"
                rules =
                    [ InSet(
                        [ "WAR"; "TWAR" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      Limit 1u
                      IsFoil false ] }
              { name = "Collection ELD"
                rules =
                    [ InSet(
                        [ "ELD"; "TELD" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      Limit 1u
                      IsFoil false ] }
              { name = "Collection THB"
                rules =
                    [ InSet(
                        [ "THB"; "TTHB" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      Limit 1u
                      IsFoil false ] }
              { name = "Collection IKO"
                rules =
                    [ InSet(
                        [ "IKO"; "TIKO" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      Limit 1u
                      IsFoil false ] }
              { name = "Collection ZNR"
                rules =
                    [ InSet(
                        [ "ZNR"; "TZNR" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      Limit 1u
                      IsFoil false ] }
              { name = "Collection KHM"
                rules =
                    [ InSet(
                        [ "KHM"; "TKHM" ]
                        |> Set.ofSeq
                        |> Set.map MagicSet.create
                      )
                      Limit 1u
                      IsFoil false ] }
              { name = "Lookup (Colorless)"
                rules =
                    [ Limit 1u
                      ColorIdentity([ [] ] |> Set.ofListList) ] }
              { name = "Lookup (White)"
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ White ] ] |> Set.ofListList) ] }
              { name = "Lookup (Blue)"
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ Blue ] ] |> Set.ofListList) ] }
              { name = "Lookup (Black)"
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ Black ] ] |> Set.ofListList) ] }
              { name = "Lookup (Red)"
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ Red ] ] |> Set.ofListList) ] }
              { name = "Lookup (Green)"
                rules =
                    [ Limit 1u
                      ColorIdentity([ [ Green ] ] |> Set.ofListList) ] }
              { name = "Lookup (Mixed 1)"
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
                rules = [ Limit 1u ] } ]

        { editLocations = false
          inventory = Map.empty
          loadInProgress = false
          locations = locations }
