module MagicCollectionHelper.AvaloniaApp.Components.Main.Update

open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

let perform (msg: Msg) (state: State): State =
    match msg with
    | Import ->
        match Import.perform () with
        | Some import ->
            import
            |> List.ofSeq
            |> setlr StateLenses.cards state
        | None -> state
    | Analyse ->
        let prefs = Preferences.create false 0.8 false

        Analyser.analyse state.setData prefs (state.cards |> Seq.ofList)
        |> String.concat Environment.NewLine
        |> setlr StateLenses.analyseText state
        |> setl StateLenses.viewMode ViewMode.Analyse
