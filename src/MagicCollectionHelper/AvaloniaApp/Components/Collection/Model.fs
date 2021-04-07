namespace MagicCollectionHelper.AvaloniaApp.Components.Collection

open Myriad.Plugins

[<Generator.Lenses("components-collection", "MagicCollectionHelper.Core.Types.Lens")>]
type State = { loadInProgress: bool }

open MagicCollectionHelper.Core.Types

type Msg =
    | ImportCollection
    | SaveCollection of DeckStatsCardEntry seq option

type Intent =
    | DoNothing
    | SaveEntries of DeckStatsCardEntry seq option

type Dispatch = Msg -> unit

module Model =
    open MagicCollectionHelper.Core

    let init () : State = { loadInProgress = false }
