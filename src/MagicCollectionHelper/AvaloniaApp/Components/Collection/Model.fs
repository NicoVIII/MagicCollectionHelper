namespace MagicCollectionHelper.AvaloniaApp.Components.Collection

open Elmish
open Myriad.Plugins

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp

[<Generator.Lenses("components-collection", "MagicCollectionHelper.Core.Types.Lens")>]
type State = { loadInProgress: bool }

[<Generator.DuCases("components-collection")>]
type Msg =
    | StartUp
    | LoadCollection
    | ImportCollection
    | WriteCollection of DeckStatsCardEntry seq option
    | SaveCollection of DeckStatsCardEntry list option

type Intent =
    | DoNothing
    | ChangeEntryState of LoadingState
    | SaveEntries of DeckStatsCardEntry list option

type Dispatch = Msg -> unit

module Model =
    let init () : State * Cmd<Msg> =
        let state = { loadInProgress = false }

        let cmd = Cmd.ofMsg StartUp

        state, cmd
