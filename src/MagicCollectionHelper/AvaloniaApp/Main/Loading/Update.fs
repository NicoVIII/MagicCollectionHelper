namespace MagicCollectionHelper.AvaloniaApp.Main.Loading

open Elmish
open SimpleOptics

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Import

module Update =
    let perform msg state =
        match msg with
        | AsyncError x -> raise x
        | StartUp ->
            let cmd =
                [ PrepareCardInfo; PrepareSetData; LoadCollection ]
                |> List.map Cmd.ofMsg
                |> Cmd.batch

            state, cmd, DoNothing
        | PrepareCardInfo ->
            CardData.prepareImportFile ()
            |> function
                | FileExists path -> state, Cmd.ofMsg (ImportCardInfo path), DoNothing
                | DownloadFile job ->
                    let state = state |> Optic.set StateOptic.cardInfo Download

                    let fnc () = job

                    let cmd = Cmd.OfAsync.either fnc () ImportCardInfo AsyncError

                    state, cmd, DoNothing
        | ImportCardInfo filePath ->
            let state = state |> Optic.set StateOptic.cardInfo Import

            let fnc () = CardData.importFile filePath

            let cmd = Cmd.OfAsync.either fnc () SaveCardInfo AsyncError

            state, cmd, DoNothing
        | SaveCardInfo import ->
            let state = state |> Optic.set StateOptic.cardInfo (Ready import)

            state, Cmd.ofMsg CheckLoadingState, DoNothing
        | PrepareSetData ->
            SetData.prepareImportFile ()
            |> function
                | FileExists path -> state, Cmd.ofMsg (ImportSetData path), DoNothing
                | DownloadFile job ->
                    let state = state |> Optic.set StateOptic.setData Download

                    let fnc () = job

                    let cmd = Cmd.OfAsync.either fnc () ImportSetData AsyncError

                    state, cmd, DoNothing
        | ImportSetData filePath ->
            let state = state |> Optic.set StateOptic.setData Import

            let fnc () = SetData.importFile filePath

            let cmd = Cmd.OfAsync.either fnc () SaveSetData AsyncError

            state, cmd, DoNothing
        | SaveSetData import ->
            let state = state |> Optic.set StateOptic.setData (Ready import)

            state, Cmd.ofMsg CheckLoadingState, DoNothing
        | LoadCollection ->
            let fnc = Persistence.DeckStatsCardEntry.loadAsync

            let cmd = Cmd.OfAsync.perform fnc () SaveCollection

            state, cmd, DoNothing
        | SaveCollection collection ->
            let state =
                let value = collection |> Option.defaultValue [] |> Ready
                Optic.set StateOptic.dsEntries value state

            state, Cmd.ofMsg CheckLoadingState, DoNothing
        | CalcEntries(cardInfo, entries) ->
            let state = state |> Optic.set StateOptic.entries Import

            let fnc () =
                DeckStatsCardEntry.listToEntriesAsync cardInfo entries

            let cmd = Cmd.OfAsync.either fnc () SaveEntries AsyncError

            state, cmd, DoNothing
        | SaveEntries entries ->
            let state = Optic.set StateOptic.entries (Ready entries) state

            state, Cmd.ofMsg CheckLoadingState, DoNothing
        | CheckLoadingState ->
            // Check how far we are in the loading process and if some calculations can already be started
            let cmd, intent =
                match state.cardInfo, state.dsEntries, state.entries, state.setData with
                | Ready cardInfo, Ready dsEntries, Ready entries, Ready setData ->
                    Cmd.none, ChangeToReady(cardInfo, dsEntries, entries, setData)
                | Ready cardInfo, Ready dsEntries, _, _ -> CalcEntries(cardInfo, dsEntries) |> Cmd.ofMsg, DoNothing
                | _ -> Cmd.none, DoNothing

            state, cmd, intent
