module MagicCollectionHelper.AvaloniaApp.Components.Collection.Update

open Elmish
open SimpleOptics

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Import

open MagicCollectionHelper.AvaloniaApp.Components.Collection

let perform (msg: Msg) (state: State) =
    match msg with
    | ImportCollection ->
        let state = Optic.set StateLenses.loadInProgress true state

        let fnc = Collection.importAsync

        let cmd = Cmd.OfAsync.perform fnc () WriteCollection

        state, cmd, DoNothing
    | WriteCollection import ->
        let entryList = Option.map List.ofSeq import

        match entryList with
        | Some entryList ->
            // And now we save the file
            Persistence.DeckStatsCardEntry.save entryList
        | None -> ()

        state, Cmd.ofMsg (SaveCollection entryList), DoNothing
    | SaveCollection import ->
        let state = Optic.set StateLenses.loadInProgress false state

        state, Cmd.none, SaveEntries import
    | ChangePage change ->
        let state = state |> Optic.map StateLenses.pageOffset change

        state, Cmd.none, DoNothing
    | SetPageSize size ->
        let state = state |> Optic.set StateLenses.pageSize size

        // Set page back to 0
        let cmd = (fun _ -> 0) |> ChangePage |> Cmd.ofMsg

        state, cmd, DoNothing
