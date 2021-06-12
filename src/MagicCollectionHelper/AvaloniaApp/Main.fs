namespace MagicCollectionHelper.AvaloniaApp.Main

open Avalonia.FuncUI.Types
open Elmish
open Myriad.Plugins

type State =
    | Loading of Loading.State
    | Ready of Ready.State

[<Generator.DuCases("main")>]
type Msg =
    | LoadingMsg of Loading.Msg
    | ReadyMsg of Ready.Msg

type Dispatch = Msg -> unit

module Model =
    let init () =
        let state, cmd = Loading.Model.init ()

        Loading state, Cmd.map LoadingMsg cmd

module Update =
    let processLoadingIntent intent state =
        match intent with
        | Loading.DoNothing -> state
        | Loading.ChangeToReady (cardInfo, dsEntries, entries, setData) ->
            Ready.Model.init cardInfo dsEntries entries setData
            |> Ready

    let perform msg state =
        match state, msg with
        | Loading state, LoadingMsg msg ->
            let state, cmd, intent = Loading.Update.perform msg state

            let state' = processLoadingIntent intent (Loading state)

            state', Cmd.map LoadingMsg cmd
        | Ready state, ReadyMsg msg ->
            let state, cmd = Ready.Update.perform msg state

            Ready state, Cmd.map ReadyMsg cmd
        | _ -> failwith "Msg from wrong state"

module View =
    let render (state: State) (dispatch: Dispatch) : IView =
        match state with
        | Loading state -> Loading.View.render state (LoadingMsg >> dispatch)
        | Ready state -> Ready.View.render state (ReadyMsg >> dispatch)
