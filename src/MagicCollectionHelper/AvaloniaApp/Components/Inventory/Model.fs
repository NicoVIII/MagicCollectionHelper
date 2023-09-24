namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory

type ViewMode =
    | Empty
    | Edit
    | Loading
    | Location of MagicCollectionHelper.Core.DomainTypes.InventoryLocation option

type LocationWithHungTree =
    MagicCollectionHelper.Core.DomainTypes.InventoryLocation *
    MagicCollectionHelper.AvaloniaApp.ViewHelper.HungTree<string, MagicCollectionHelper.Core.CardTypes.AgedEntryWithInfo list>

type State = {
    filteredInventory: LocationWithHungTree list
    inventory: MagicCollectionHelper.Core.DomainTypes.LocationWithCards
    locations: MagicCollectionHelper.Core.DomainTypes.CustomLocation list
    search: MagicCollectionHelper.AvaloniaApp.DomainTypes.Search
    viewMode: ViewMode
}

open MagicCollectionHelper.Core

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
    open Elmish

    let init () : State * Cmd<'a> =
        // Test Locations
        let locations =
            Persistence.CustomLocation.load ()
            |> Async.RunSynchronously // TODO: Move to loading step or implement loading for inventory step
            |> Option.defaultValue []

        {
            inventory = []
            filteredInventory = []
            locations = locations
            search = { text = ""; old = None }
            viewMode = Empty
        },
        Cmd.none
