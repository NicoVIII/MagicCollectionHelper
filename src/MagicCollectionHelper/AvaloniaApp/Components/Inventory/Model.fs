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
          [ Rare; Mythic; Special; Bonus ] ]
        |> List.map Set.ofList
        |> ByRarity

    let init () : State =
        // Test Locations
        let locations =
            Persistence.RawCustomLocation.load ()
            |> Option.defaultValue []
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
