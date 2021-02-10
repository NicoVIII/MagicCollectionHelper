module MagicCollectionHelper.AvaloniaApp.Components.Main.Update

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
