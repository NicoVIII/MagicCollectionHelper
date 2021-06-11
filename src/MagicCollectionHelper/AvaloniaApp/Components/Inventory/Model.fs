namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory

open Myriad.Plugins

type ViewMode =
    | Empty
    | Edit
    | Loading
    | Location of MagicCollectionHelper.Core.Types.InventoryLocation option

type LocationWithHungTree =
    MagicCollectionHelper.Core.Types.InventoryLocation * MagicCollectionHelper.AvaloniaApp.ViewHelper.HungTree<string, MagicCollectionHelper.Core.Types.OldAmountable<MagicCollectionHelper.Core.Types.CardEntryWithInfo> list>

[<Generator.Lenses("components.inventory", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { filteredInventory: LocationWithHungTree list
      inventory: MagicCollectionHelper.Core.Types.LocationWithCards
      locations: MagicCollectionHelper.Core.Types.CustomLocation list
      search: MagicCollectionHelper.AvaloniaApp.DomainTypes.Search
      viewMode: ViewMode }

open MagicCollectionHelper.Core.Types

[<Generator.DuCases("components.inventory")>]
type Msg =
    | AsyncError of exn
    | ChangeState of (State -> State)
    | TakeInventory
    | SaveInventory of LocationWithCards
    | OpenLocationEdit
    | CloseLocationEdit
    | FilterInventory of LocationWithCards
    | ChangeLocation of InventoryLocation
    | UpdateLocationRules of CustomLocationName * (Rules -> Rules)

type Dispatch = Msg -> unit

module Model =
    open MagicCollectionHelper.Core

    let init () : State =
        // Test Locations
        let locations =
            Persistence.CustomLocation.load ()
            |> Option.defaultValue []

        { inventory = []
          filteredInventory = []
          locations = locations
          search = { text = ""; old = None }
          viewMode = Empty }
