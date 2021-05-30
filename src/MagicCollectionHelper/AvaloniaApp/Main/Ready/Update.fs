namespace MagicCollectionHelper.AvaloniaApp.Main.Ready

open Elmish
open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Import
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components

module Update =
    let processCollectionIntent intent (state, cmd) =
        match intent with
        | Collection.Intent.SaveEntries entries ->
            let state =
                state
                |> match entries with
                   | Some entries -> setl StateLenses.dsEntries entries
                   | None -> id

            state, Cmd.none
        | Collection.Intent.DoNothing -> state, cmd // We don't need to mutate the state

    let perform (msg: Msg) (state: State) =
        match msg with
        | AsyncError x -> raise x
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

            state, Cmd.ofMsg (SavePrefs prefs)
        | SavePrefs prefs ->
            Persistence.Prefs.save prefs

            state, Cmd.none
        | CollectionMsg msg ->
            let (iState, iCmd, intent) =
                Collection.Update.perform msg state.collection

            (state, iCmd |> Cmd.map CollectionMsg)
            |> Tuple2.mapFst (setl StateLenses.collection iState)
            |> processCollectionIntent intent
        | InventoryMsg msg ->
            let cardInfo = getl StateLenses.cardInfo state
            let setData = getl StateLenses.setData state

            let entries = getl StateLenses.entries state

            let (iState, iCmd) =
                Inventory.Update.perform setData cardInfo entries msg state.inventory

            let state = setl StateLenses.inventory iState state
            let cmd = iCmd |> Cmd.map InventoryMsg

            state, cmd
