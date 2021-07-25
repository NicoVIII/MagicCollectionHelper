module MagicCollectionHelper.AvaloniaApp.Components.Collection.Update

open Elmish

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Import

open MagicCollectionHelper.AvaloniaApp.Components.Collection.Generated

let perform (msg: Msg) (state: State) =
    match msg with
    | ImportCollection ->
        let state = setl StateLenses.loadInProgress true state

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
        let state = setl StateLenses.loadInProgress false state

        state, Cmd.none, SaveEntries import
    | ChangePage change ->
        let state = state |> StateLenses.pageOffset %-> change

        state, Cmd.none, DoNothing
    | SetPageSize size ->
        let state = state |> StateLenses.pageSize .-> size

        // Set page back to 0
        let cmd = (fun _ -> 0) |> ChangePage |> Cmd.ofMsg

        state, cmd, DoNothing
