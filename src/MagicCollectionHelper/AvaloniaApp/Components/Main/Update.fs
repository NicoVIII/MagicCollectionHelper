module MagicCollectionHelper.AvaloniaApp.Components.Main.Update

open System

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components

let perform (msg: Msg) (state: State): State =
    match msg with
    | ImportCollection ->
        match CollectionImport.perform () with
        | Some import ->
            import
            |> List.ofSeq
            |> setlr StateL.entries state
        | None -> state
    | Analyse ->
        let prefs = getl StateL.prefs state
        let setData = getl StateL.setData state
        let entries = getl StateL.entries state

        Analyser.analyse setData prefs (entries |> Seq.ofList)
        |> String.concat Environment.NewLine
        |> setlr StateL.analyseText state
    | ChangeViewMode viewMode ->
        setl StateL.viewMode viewMode state
    | ChangePrefs prefs ->
        setl StateL.prefs prefs state
    | InventoryMsg msg ->
        let entries = getl StateL.entries state

        Inventory.Update.perform entries msg state.inventory
        |> setlr StateLenses.inventory state
