namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory

open Myriad.Plugins
open System

type LocationCardMap =
    Map<MagicCollectionHelper.Core.Types.InventoryLocation, MagicCollectionHelper.Core.Types.CardEntry list>

[<Generator.Lenses("components-inventory", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { inventory: LocationCardMap
      locations: MagicCollectionHelper.Core.Types.CustomLocation list }

type Msg = | TakeInventory

type Dispatch = Msg -> unit

module Model =
    open MagicCollectionHelper.Core.Types

    let init: State =
        // Test Locations
        let locations = [
            { name = "Collection GRN"
              rules = [ InSet [MagicSet "GRN"]; Limit 1u ] }
            { name = "Collection RNA"
              rules = [ InSet [MagicSet "RNA"]; Limit 1u ] }
            { name = "Collection WAR"
              rules = [ InSet [MagicSet "WAR"]; Limit 1u ] }
            { name = "Collection ELD"
              rules = [ InSet [MagicSet "ELD"]; Limit 1u ] }
            { name = "Collection THB"
              rules = [ InSet [MagicSet "THB"]; Limit 1u ] }
            { name = "Collection IKO"
              rules = [ InSet [MagicSet "IKO"]; Limit 1u ] }
            { name = "Collection ZNR"
              rules = [ InSet [MagicSet "ZNR"]; Limit 1u ] }
            { name = "Collection KHM"
              rules = [ InSet [MagicSet "KHM"]; Limit 1u ] }
            { name = "Lookup"
              rules = [ Limit 1u ] }
        ]

        { inventory = Map.empty
          locations = locations }
