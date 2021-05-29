module MagicCollectionHelper.AvaloniaApp.Main.Update

open Elmish
open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Import
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components
open MagicCollectionHelper.AvaloniaApp.Generated
open MagicCollectionHelper.AvaloniaApp.Main
open MagicCollectionHelper.AvaloniaApp.Main.Generated

let processCollectionIntent intent (state, cmd) =
    match intent with
    | Collection.Intent.SaveEntries entries ->
        let state =
            state
            |> setl StateLenses.dsEntriesState Ready
            |> match entries with
               | Some entries -> setl StateLenses.dsEntries entries
               | None -> id

        let cmd =
            Cmd.batch [
                cmd
                Cmd.ofMsg CheckLoadingState
            ]

        state, cmd
    | Collection.Intent.ChangeEntryState loadingState ->
        let state =
            state
            |> setl StateLenses.dsEntriesState loadingState

        state, cmd
    | Collection.Intent.DoNothing -> state, cmd // We don't need to mutate the state

let perform (msg: Msg) (state: State) =
    match msg with
    | AsyncError x -> raise x
    | StartUp ->
        let cmd =
            [ PrepareCardInfo; PrepareSetData ]
            |> List.map Cmd.ofMsg
            |> Cmd.batch

        state, cmd
    | PrepareCardInfo ->
        CardData.prepareImportFile ()
        |> function
        | FileExists path -> state, ImportCardInfo path |> Cmd.ofMsg
        | DownloadFile job ->
            let state =
                state |> setl StateLenses.cardInfoState Download

            let fnc () = job

            let cmd =
                Cmd.OfAsync.either fnc () ImportCardInfo AsyncError

            state, cmd
    | ImportCardInfo filePath ->
        let state =
            state |> setl StateLenses.cardInfoState Import

        let fnc () = CardData.importFile filePath

        let cmd =
            Cmd.OfAsync.either fnc () SaveCardInfo AsyncError

        state, cmd
    | SaveCardInfo import ->
        let state =
            state
            |> setl StateLenses.cardInfo import
            |> setl StateLenses.cardInfoState Ready

        state, Cmd.ofMsg CheckLoadingState
    | PrepareSetData ->
        SetData.prepareImportFile ()
        |> function
        | FileExists path -> state, ImportSetData path |> Cmd.ofMsg
        | DownloadFile job ->
            let state =
                state |> setl StateLenses.setDataState Download

            let fnc () = job

            let cmd =
                Cmd.OfAsync.either fnc () ImportSetData AsyncError

            state, cmd
    | ImportSetData filePath ->
        let state =
            state |> setl StateLenses.setDataState Import

        let fnc () = SetData.importFile filePath

        let cmd =
            Cmd.OfAsync.either fnc () SaveSetData AsyncError

        state, cmd
    | SaveSetData import ->
        let state =
            state
            |> setl StateLenses.setData import
            |> setl StateLenses.setDataState Ready

        state, Cmd.ofMsg CheckLoadingState
    | CheckLoadingState ->
        // Check if everything is loaded and change viewmode if that is the case
        let setDataState = getl StateLenses.setDataState state
        let cardInfoState = getl StateLenses.cardInfoState state
        let dsEntriesState = getl StateLenses.dsEntriesState state

        let allReady =
            [ setDataState
              cardInfoState
              dsEntriesState ]
            |> List.forall LoadingState.isReady

        let cmd =
            if allReady then
                Cmd.ofMsg CalcEntries
            else
                Cmd.none

        state, cmd
    | CalcEntries ->
        let cardInfo = getl StateLenses.cardInfo state
        let entries = getl StateLenses.dsEntries state

        let state =
            state |> setl StateLenses.entriesState Import

        let fnc () =
            DeckStatsCardEntry.listToEntriesAsync cardInfo entries

        let cmd =
            Cmd.OfAsync.either fnc () SaveEntries AsyncError

        state, cmd
    | SaveEntries entries ->
        let state =
            state
            |> setl StateLenses.entries entries
            |> setl StateLenses.entriesState Ready

        state, Cmd.ofMsg (ChangeViewMode Collection)
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
