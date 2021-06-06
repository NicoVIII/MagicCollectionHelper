namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory

open Myriad.Plugins

type LocationCardMap =
    Map<MagicCollectionHelper.Core.Types.InventoryLocation, MagicCollectionHelper.Core.Types.CardEntry list>

type ViewMode =
    | Empty
    | Edit
    | Loading
    | Location of MagicCollectionHelper.Core.Types.InventoryLocation option

[<Generator.Lenses("components.inventory", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { filteredInventory: (MagicCollectionHelper.Core.Types.InventoryLocation * MagicCollectionHelper.Core.Types.CardEntryWithInfo list) list
      inventory: LocationCardMap
      locations: Map<MagicCollectionHelper.Core.Types.CustomLocationName, MagicCollectionHelper.Core.Types.CustomLocation>
      search: string
      viewMode: ViewMode }

open MagicCollectionHelper.Core.Types

[<Generator.DuCases("components.inventory")>]
type Msg =
    | AsyncError of exn
    | TakeInventory
    | SaveInventory of LocationCardMap
    | OpenLocationEdit
    | CloseLocationEdit
    | ChangeSearchString of string
    | Search
    | FilterInventory of LocationCardMap
    | ChangeLocation of InventoryLocation
    | UpdateLocationRules of CustomLocationName * (Rules -> Rules)

type Dispatch = Msg -> unit

module Model =
    open MagicCollectionHelper.Core

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

        { inventory = Map.empty
          filteredInventory = []
          locations = locations
          search = ""
          viewMode = Empty }
