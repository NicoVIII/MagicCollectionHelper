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
    open MagicCollectionHelper.Core.Types

    let init (): State =
        // Test Locations
        let locations = [
            { name = "Collection GRN"
              rules = [ InSet (MagicSet.create "GRN"); Limit 1u; IsFoil false ] }
            { name = "Collection RNA"
              rules = [ InSet (MagicSet.create "RNA"); Limit 1u; IsFoil false ] }
            { name = "Collection WAR"
              rules = [ InSet (MagicSet.create "WAR"); Limit 1u; IsFoil false ] }
            { name = "Collection ELD"
              rules = [ InSet (MagicSet.create "ELD"); Limit 1u; IsFoil false ] }
            { name = "Collection THB"
              rules = [ InSet (MagicSet.create "THB"); Limit 1u; IsFoil false ] }
            { name = "Collection IKO"
              rules = [ InSet (MagicSet.create "IKO"); Limit 1u; IsFoil false ] }
            { name = "Collection ZNR"
              rules = [ InSet (MagicSet.create "ZNR"); Limit 1u; IsFoil false ] }
            { name = "Collection KHM"
              rules = [ InSet (MagicSet.create "KHM"); Limit 1u; IsFoil false ] }
            { name = "Lookup"
              rules = [ Limit 1u ] }
        ]

        { editLocations = false
          inventory = Map.empty
          loadInProgress = false
          locations = locations }
