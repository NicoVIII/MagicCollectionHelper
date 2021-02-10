module MagicCollectionHelper.AvaloniaApp.Components.Main.Update

open MagicCollectionHelper.Core

let perform (msg: Msg) (state: State): State =
    match msg with
    | ImportFromFile ->
        match Import.perform () with
        | Some import -> { cards = import |> List.ofSeq }
        | None -> state
