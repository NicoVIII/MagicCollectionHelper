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
                let state = state |> Optic.set StateOptic.dsEntries entries

                let fnc () =
                    let cardInfo = Optic.get StateOptic.cardInfo state
                    DeckStatsCardEntry.listToEntriesAsync cardInfo entries

                let cmd = Cmd.OfAsync.either fnc () SaveEntries AsyncError

                state, cmd
            | None -> state, Cmd.none
        | Collection.Intent.DoNothing -> state, cmd // We don't need to mutate the state

    let perform (msg: Msg) (state: State) =
        match msg with
        | AsyncError x -> raise x
        | ChangeViewMode viewMode ->
            let state = Optic.set StateOptic.viewMode viewMode state

            state, Cmd.none
        | ChangePrefs prefs ->
            let state = (Optic.map StateOptic.prefs prefs) state

            state, Cmd.ofMsg SavePrefs
        | SavePrefs ->
            state ^. StateOptic.prefs |> Persistence.Prefs.save

            state, Cmd.none
        | SaveEntries entries ->
            let oldEntries =
                state
                |> Optic.get StateOptic.entries
                |> List.map (Optic.get AgedEntryOptic.entry)

            let comparedEntries = AgedEntry.determineCardAge oldEntries entries

            let state = state |> Optic.set StateOptic.entries comparedEntries

            state, Cmd.none
        | CollectionMsg msg ->
            let (iState, iCmd, intent) = Collection.Update.perform msg state.collection

            (state, iCmd |> Cmd.map CollectionMsg)
            |> Tuple2.mapFst (Optic.set StateOptic.collection iState)
            |> processCollectionIntent intent
        | InventoryMsg msg ->
            let cardInfo = Optic.get StateOptic.cardInfo state
            let setData = Optic.get StateOptic.setData state
            let entries = Optic.get StateOptic.entries state
            let prefs = Optic.get StateOptic.prefs state

            let (iState, iCmd) =
                Inventory.Update.perform prefs setData cardInfo entries msg state.inventory

            let state = Optic.set StateOptic.inventory iState state
            let cmd = iCmd |> Cmd.map InventoryMsg

            state, cmd
