module MagicCollectionHelper.AvaloniaApp.Components.Collection.Update

open Elmish
open SimpleOptics

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Import

open MagicCollectionHelper.AvaloniaApp.Components.Collection

let perform (msg: Msg) (state: State) =
    match msg with
    | AsyncError x -> raise x
    | ImportCollection ->
        let state = Optic.set StateOptic.loadInProgress true state

        let fnc = Collection.importAsync

        let cmd = Cmd.OfAsync.either fnc () WriteCollection AsyncError

        state, cmd, DoNothing
    | WriteCollection import ->
        let entryList = Option.map List.ofSeq import

        let cmd =
            Cmd.batch [
                Cmd.ofMsg (SaveCollection entryList)
                match entryList with
                | Some entryList ->
                    // And now we save the file
                    Cmd.OfAsync.attempt Persistence.DeckStatsCardEntry.save entryList AsyncError
                | None -> ()
            ]

        state, cmd, DoNothing
    | SaveCollection import ->
        let state = Optic.set StateOptic.loadInProgress false state

        state, Cmd.none, SaveEntries import
    | ChangePage change ->
        let state = state |> Optic.map StateOptic.pageOffset change

        state, Cmd.none, DoNothing
    | SetPageSize size ->
        let state = state |> Optic.set StateOptic.pageSize size

        // Set page back to 0
        let cmd = (fun _ -> 0) |> ChangePage |> Cmd.ofMsg

        state, cmd, DoNothing
