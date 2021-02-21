module MagicCollectionHelper.AvaloniaApp.Components.Main.Update

open Elmish
open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components

let perform (msg: Msg) (state: State) =
    match msg with
    | ImportCollection ->
        let state = setl StateL.loadInProgress true state
        let fnc () = CollectionImport.performAsync ()
        let cmd = Cmd.OfAsync.perform fnc () SaveCollection

        (state, cmd)
    | SaveCollection import ->
        let state =
            match import with
            | Some import ->
                import
                |> List.ofSeq
                |> setlr StateL.entries state
            | None -> state
            |> setl StateL.loadInProgress false

        (state, Cmd.none)
    | Analyse ->
        let prefs = getl StateL.prefs state
        let setData = getl StateL.setData state
        let entries = getl StateL.entries state

        let state =
            Analyser.analyse setData prefs (entries |> Seq.ofList)
            |> String.concat Environment.NewLine
            |> setlr StateL.analyseText state

        (state, Cmd.none)
    | ChangeViewMode viewMode ->
        let state = setl StateL.viewMode viewMode state

        (state, Cmd.none)
    | ChangePrefs prefs ->
        let state = setl StateL.prefs prefs state

        (state, Cmd.none)
    | InventoryMsg msg ->
        let entries = getl StateL.entries state

        let (iState, iCmd) =
            Inventory.Update.perform entries msg state.inventory

        let state = setl StateLenses.inventory iState state
        let cmd = iCmd |> Cmd.map InventoryMsg

        (state, cmd)
