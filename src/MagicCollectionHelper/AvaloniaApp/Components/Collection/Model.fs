namespace MagicCollectionHelper.AvaloniaApp.Components.Collection

open Myriad.Plugins

open MagicCollectionHelper.Core.Types

[<Generator.Lenses("components-collection", "MagicCollectionHelper.Core.Types.Lens")>]
type State = { loadInProgress: bool }

[<Generator.DuCases("components-collection")>]
type Msg =
    | ImportCollection
    | WriteCollection of DeckStatsCardEntry seq option
    | SaveCollection of DeckStatsCardEntry list option

type Intent =
    | DoNothing
    | SaveEntries of DeckStatsCardEntry list option

type Dispatch = Msg -> unit

module Model =
    let init () = { loadInProgress = false }
