module MagicCollectionHelper.AvaloniaApp.Components.Main.Update

open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

let perform (msg: Msg) (state: State): State =
    match msg with
    | ImportFromFile ->
        match Import.perform () with
        | Some import ->
            import
            |> List.ofSeq
            |> setlr StateLenses.cards state
        | None -> state
    | Analyse ->
        let settings = Settings.create false 0.8 false

        Analyser.analyse state.setData settings (state.cards |> Seq.ofList)
        |> String.concat Environment.NewLine
        |> setlr StateLenses.text state
