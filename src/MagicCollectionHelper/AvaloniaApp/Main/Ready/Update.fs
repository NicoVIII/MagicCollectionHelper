namespace MagicCollectionHelper.AvaloniaApp.Main.Ready

open Elmish
open SimpleOptics
open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Import

open MagicCollectionHelper.AvaloniaApp.Components

module Update =
    let processCollectionIntent intent (state, cmd) =
        match intent with
        | Collection.Intent.SaveEntries entries ->
            match entries with
            | Some entries ->
                let state = state |> Optic.set StateLenses.dsEntries entries

                let fnc () =
                    let cardInfo = Optic.get StateLenses.cardInfo state
                    DeckStatsCardEntry.listToEntriesAsync cardInfo entries

                let cmd = Cmd.OfAsync.either fnc () SaveEntries AsyncError

                state, cmd
            | None -> state, Cmd.none
        | Collection.Intent.DoNothing -> state, cmd // We don't need to mutate the state

    let perform (msg: Msg) (state: State) =
        match msg with
        | AsyncError x -> raise x
        | Analyse ->
            let prefs = Optic.get StateLenses.prefs state
            let setData = Optic.get StateLenses.setData state
            let entries = Optic.get StateLenses.dsEntries state

            let state =
                let value =
                    Analyser.analyse setData prefs (entries |> Seq.ofList)
                    |> String.concat Environment.NewLine

                Optic.set StateLenses.analyseText value state

            state, Cmd.none
        | ChangeViewMode viewMode ->
            let state = Optic.set StateLenses.viewMode viewMode state

            state, Cmd.none
        | ChangePrefs prefs ->
            let state = (Optic.map StateLenses.prefs prefs) state

            state, Cmd.ofMsg SavePrefs
        | SavePrefs ->
            state ^. StateLenses.prefs
            |> Persistence.Prefs.save

            state, Cmd.none
        | SaveEntries entries ->
            let oldEntries =
                state
                |> Optic.get StateLenses.entries
                |> List.map (Optic.get AgedEntryLenses.entry)

            let comparedEntries = AgedEntry.determineCardAge oldEntries entries

            let state =
                state
                |> Optic.set StateLenses.entries comparedEntries

            state, Cmd.none
        | CollectionMsg msg ->
            let (iState, iCmd, intent) = Collection.Update.perform msg state.collection

            (state, iCmd |> Cmd.map CollectionMsg)
            |> Tuple2.mapFst (Optic.set StateLenses.collection iState)
            |> processCollectionIntent intent
        | InventoryMsg msg ->
            let cardInfo = Optic.get StateLenses.cardInfo state
            let setData = Optic.get StateLenses.setData state
            let entries = Optic.get StateLenses.entries state
            let prefs = Optic.get StateLenses.prefs state

            let (iState, iCmd) =
                Inventory.Update.perform prefs setData cardInfo entries msg state.inventory

            let state = Optic.set StateLenses.inventory iState state
            let cmd = iCmd |> Cmd.map InventoryMsg

            state, cmd
