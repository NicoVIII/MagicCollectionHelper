module MagicCollectionHelper.AvaloniaApp.Components.Main.Update

open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

let perform (msg: Msg) (state: State): State =
    match msg with
    | ImportCollection ->
        match CollectionImport.perform () with
        | Some import ->
            import
            |> List.ofSeq
            |> setlr StateLenses.cards state
        | None -> state
    | Analyse ->
        Analyser.analyse state.setData state.prefs (state.cards |> Seq.ofList)
        |> String.concat Environment.NewLine
        |> setlr StateLenses.analyseText state
    | ChangeViewMode viewMode ->
        setl StateLenses.viewMode viewMode state
    | ChangePrefs prefs ->
        setl StateLenses.prefs prefs state
    | TakeInventory ->
        Inventory.take state.locations state.cards
        |> setlr StateLenses.inventory state
