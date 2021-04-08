module MagicCollectionHelper.AvaloniaApp.Main.Update

open Elmish
open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components
open MagicCollectionHelper.AvaloniaApp.Main
open MagicCollectionHelper.AvaloniaApp.Main.Generated

let processCollectionIntent intent stateCmd =
    match intent with
    | Collection.Intent.SaveEntries entries ->
        stateCmd
        |> Tuple2.mapFst
            (fun state ->
                match entries with
                | Some entries -> setl StateLenses.dsEntries (Seq.toList entries) state
                | None -> state)
        |> Tuple2.mapSnd (fun cmd -> Cmd.batch [ cmd; Cmd.ofMsg CalcEntries ])
    | Collection.Intent.DoNothing -> stateCmd // We don't need to mutate the state

let perform (msg: Msg) (state: State) =
    match msg with
    | AsyncError x -> raise x
    | CalcEntries ->
        let infoMap = getl StateLenses.infoMap state
        let entries = getl StateLenses.dsEntries state

        let fnc () =
            DeckStatsCardEntry.listToEntriesAsync infoMap entries

        let cmd =
            Cmd.OfAsync.either fnc () SaveEntries AsyncError

        (state, cmd)
    | SaveEntries entries ->
        let state = setl StateLenses.entries entries state

        (state, Cmd.none)
    | ImportCardInfo ->
        let fnc = CardDataImport.performAsync

        let cmd =
            Cmd.OfAsync.either fnc () SaveCardInfo AsyncError

        state, cmd
    | SaveCardInfo import ->
        let state =
            match import with
            | Some import -> import |> setlr StateLenses.infoMap state
            | None -> state

        state, Cmd.none
    | Analyse ->
        let prefs = getl StateLenses.prefs state
        let setData = getl StateLenses.setData state
        let entries = getl StateLenses.dsEntries state

        let state =
            Analyser.analyse setData prefs (entries |> Seq.ofList)
            |> String.concat Environment.NewLine
            |> setlr StateLenses.analyseText state

        state, Cmd.none
    | ChangeViewMode viewMode ->
        let state = setl StateLenses.viewMode viewMode state

        state, Cmd.none
    | ChangePrefs prefs ->
        let state = setl StateLenses.prefs prefs state

        state, Cmd.none
    | CollectionMsg msg ->
        let (iState, iCmd, intent) =
            Collection.Update.perform msg state.collection

        (state, iCmd |> Cmd.map CollectionMsg)
        |> Tuple2.mapFst (setl StateLenses.collection iState)
        |> processCollectionIntent intent
    | InventoryMsg msg ->
        let infoMap = getl StateLenses.infoMap state

        let entries = getl StateLenses.entries state

        let infoMap = getl StateLenses.infoMap state

        let (iState, iCmd) =
            Inventory.Update.perform infoMap entries msg state.inventory

        let state = setl StateLenses.inventory iState state
        let cmd = iCmd |> Cmd.map InventoryMsg

        state, cmd
