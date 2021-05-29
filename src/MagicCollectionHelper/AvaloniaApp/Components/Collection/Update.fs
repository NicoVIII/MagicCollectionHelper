module MagicCollectionHelper.AvaloniaApp.Components.Collection.Update

open Elmish

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Import
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Collection
open MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated

let perform (msg: Msg) (state: State) =
    match msg with
    | StartUp ->
        let cmd =
            [ LoadCollection ]
            |> List.map Cmd.ofMsg
            |> Cmd.batch

        state, cmd, DoNothing
    | ImportCollection ->
        let state =
            setl StateLenses.loadInProgress true state

        let fnc = Collection.importAsync

        let cmd =
            Cmd.OfAsync.perform fnc () WriteCollection

        state, cmd, DoNothing
    | LoadCollection ->
        let fnc = Persistence.DeckStatsCardEntry.loadAsync

        let cmd =
            Cmd.OfAsync.perform fnc () SaveCollection

        state, cmd, ChangeEntryState Import
    | WriteCollection import ->
        let entryList = Option.map List.ofSeq import

        match entryList with
        | Some entryList ->
            // And now we save the file
            Persistence.DeckStatsCardEntry.save entryList
        | None -> ()

        state, Cmd.ofMsg (SaveCollection entryList), DoNothing
    | SaveCollection import ->
        let state =
            setl StateLenses.loadInProgress false state

        state, Cmd.none, SaveEntries import
