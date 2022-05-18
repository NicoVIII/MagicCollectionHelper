namespace MagicCollectionHelper.AvaloniaApp.Components.Collection

open Myriad.Plugins

open MagicCollectionHelper.Core

[<Generator.Lenses("components-collection", "SimpleOptics.Lens")>]
type State =
    {
        loadInProgress: bool
        pageSize: int
        pageOffset: int
    }

[<Generator.DuCases("components-collection")>]
type Msg =
    | ImportCollection
    | WriteCollection of DeckStatsCardEntry seq option
    | SaveCollection of DeckStatsCardEntry list option
    | ChangePage of (int -> int)
    | SetPageSize of int

type Intent =
    | DoNothing
    | SaveEntries of DeckStatsCardEntry list option

type Dispatch = Msg -> unit

module Model =
    let init () =
        {
            loadInProgress = false
            pageSize = 50
            pageOffset = 0
        }
