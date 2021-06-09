//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.AvaloniaApp.Main.Generated

module Msg =
    open MagicCollectionHelper.AvaloniaApp.Main
    let toString (x: Msg) =
        match x with
        | LoadingMsg _ -> "LoadingMsg"
        | ReadyMsg _ -> "ReadyMsg"

    let fromString (x: string) =
        match x with
        | _ -> None

    let toTag (x: Msg) =
        match x with
        | LoadingMsg _ -> 0
        | ReadyMsg _ -> 1

    let isLoadingMsg (x: Msg) =
        match x with
        | LoadingMsg _ -> true
        | _ -> false

    let isReadyMsg (x: Msg) =
        match x with
        | ReadyMsg _ -> true
        | _ -> false