module MagicCollectionHelper.AvaloniaApp.Components.Collection.Update

open Elmish

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components.Collection
open MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated

let perform (msg: Msg) (state: State) =
    match msg with
    | ImportCollection ->
        let state =
            setl StateLenses.loadInProgress true state

        let fnc = CollectionImport.performAsync

        let cmd =
            Cmd.OfAsync.perform fnc () SaveCollection

        state, cmd, DoNothing
    | SaveCollection import ->
        let state =
            setl StateLenses.loadInProgress false state

        state, Cmd.none, SaveEntries import
