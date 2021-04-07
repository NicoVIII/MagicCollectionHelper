module MagicCollectionHelper.AvaloniaApp.Components.Main.Update

open Elmish
open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components
open MagicCollectionHelper.AvaloniaApp.Components.Main
open MagicCollectionHelper.AvaloniaApp.Components.Main.Generated

let perform (msg: Msg) (state: State) =
    match msg with
    | ImportCardInfo ->
        let fnc = CardDataImport.performAsync

        let cmd =
            Cmd.OfAsync.either fnc () SaveCardInfo (fun x -> raise x)

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
        let entries = getl StateLenses.entries state

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
        let processIntent intent state =
            match intent with
            | Collection.Intent.SaveEntries entries ->
                match entries with
                | Some entries -> setl StateLenses.entries (Seq.toList entries) state
                | None -> state
            | Collection.Intent.DoNothing -> state // We don't need to mutate the state

        let (iState, iCmd, intent) =
            Collection.Update.perform msg state.collection

        let state =
            state
            |> setl StateLenses.collection iState
            |> processIntent intent

        let cmd = iCmd |> Cmd.map CollectionMsg

        (state, cmd)
    | InventoryMsg msg ->
        let entries =
            getl StateLenses.entries state
            |> DeckStatsCardEntry.listToEntries

        let infoMap = getl StateLenses.infoMap state

        let (iState, iCmd) =
            Inventory.Update.perform infoMap entries msg state.inventory

        let state = setl StateLenses.inventory iState state
        let cmd = iCmd |> Cmd.map InventoryMsg

        state, cmd
