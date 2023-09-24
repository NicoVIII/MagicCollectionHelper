namespace MagicCollectionHelper.AvaloniaApp.Components.Collection

open MagicCollectionHelper.Core

type State = {
    loadInProgress: bool
    pageSize: int
    pageOffset: int
}

type Msg =
    | AsyncError of exn
    | ImportCollection
    | WriteCollection of DeckStatsCardEntry seq option
    | SaveCollection of DeckStatsCardEntry list option
    | ChangePage of (int -> int)
    | SetPageSize of int

type Dispatch = Msg -> unit

module Model =
    open Elmish

    let init () =
        {
            loadInProgress = false
            pageSize = 50
            pageOffset = 0
        },
        Cmd.none
