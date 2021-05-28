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
        let cmd =
            match import with
            | Some import ->
                let entryList = List.ofSeq import

                // And now we save the file
                Persistence.DeckStatsCardEntry.save entryList

                Cmd.ofMsg (Some entryList |> SaveCollection)
            | None -> Cmd.none

        state, cmd, DoNothing
    | SaveCollection import ->
        let state =
            setl StateLenses.loadInProgress false state

        let intent =
            match import with
            | Some import -> SaveEntries import
            | None -> DoNothing

        state, Cmd.none, intent
