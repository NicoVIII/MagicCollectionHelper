//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.AvaloniaApp.Main.Loading.Generated

module StateLenses =
    open MagicCollectionHelper.AvaloniaApp.Main.Loading

    let cardInfo =
        SimpleOptics.Lens(
            (fun (x: State) -> x.cardInfo),
            (fun (x: State) (value: Loadable<MagicCollectionHelper.Core.CardTypes.CardInfoMap>) ->
                { x with cardInfo = value })
        )

    let dsEntries =
        SimpleOptics.Lens(
            (fun (x: State) -> x.dsEntries),
            (fun (x: State) (value: Loadable<MagicCollectionHelper.Core.DomainTypes.DeckStatsCardEntry list>) ->
                { x with dsEntries = value })
        )

    let entries =
        SimpleOptics.Lens(
            (fun (x: State) -> x.entries),
            (fun (x: State) (value: Loadable<MagicCollectionHelper.Core.CardTypes.Entry list>) ->
                { x with entries = value })
        )

    let setData =
        SimpleOptics.Lens(
            (fun (x: State) -> x.setData),
            (fun (x: State) (value: Loadable<MagicCollectionHelper.Core.DomainTypes.SetDataMap>) ->
                { x with setData = value })
        )


namespace rec MagicCollectionHelper.AvaloniaApp.Main.Loading.Generated

module Msg =
    open MagicCollectionHelper.AvaloniaApp.Main.Loading

    let toString (x: Msg) =
        match x with
        | AsyncError _ -> "AsyncError"
        | StartUp -> "StartUp"
        | PrepareCardInfo -> "PrepareCardInfo"
        | ImportCardInfo _ -> "ImportCardInfo"
        | SaveCardInfo _ -> "SaveCardInfo"
        | LoadCollection -> "LoadCollection"
        | SaveCollection _ -> "SaveCollection"
        | PrepareSetData -> "PrepareSetData"
        | ImportSetData _ -> "ImportSetData"
        | SaveSetData _ -> "SaveSetData"
        | CalcEntries _ -> "CalcEntries"
        | SaveEntries _ -> "SaveEntries"
        | CheckLoadingState -> "CheckLoadingState"

    let fromString (x: string) =
        match x with
        | "StartUp" -> Some StartUp
        | "PrepareCardInfo" -> Some PrepareCardInfo
        | "LoadCollection" -> Some LoadCollection
        | "PrepareSetData" -> Some PrepareSetData
        | "CheckLoadingState" -> Some CheckLoadingState
        | _ -> None

    let toTag (x: Msg) =
        match x with
        | AsyncError _ -> 0
        | StartUp -> 1
        | PrepareCardInfo -> 2
        | ImportCardInfo _ -> 3
        | SaveCardInfo _ -> 4
        | LoadCollection -> 5
        | SaveCollection _ -> 6
        | PrepareSetData -> 7
        | ImportSetData _ -> 8
        | SaveSetData _ -> 9
        | CalcEntries _ -> 10
        | SaveEntries _ -> 11
        | CheckLoadingState -> 12

    let isAsyncError (x: Msg) =
        match x with
        | AsyncError _ -> true
        | _ -> false

    let isStartUp (x: Msg) =
        match x with
        | StartUp -> true
        | _ -> false

    let isPrepareCardInfo (x: Msg) =
        match x with
        | PrepareCardInfo -> true
        | _ -> false

    let isImportCardInfo (x: Msg) =
        match x with
        | ImportCardInfo _ -> true
        | _ -> false

    let isSaveCardInfo (x: Msg) =
        match x with
        | SaveCardInfo _ -> true
        | _ -> false

    let isLoadCollection (x: Msg) =
        match x with
        | LoadCollection -> true
        | _ -> false

    let isSaveCollection (x: Msg) =
        match x with
        | SaveCollection _ -> true
        | _ -> false

    let isPrepareSetData (x: Msg) =
        match x with
        | PrepareSetData -> true
        | _ -> false

    let isImportSetData (x: Msg) =
        match x with
        | ImportSetData _ -> true
        | _ -> false

    let isSaveSetData (x: Msg) =
        match x with
        | SaveSetData _ -> true
        | _ -> false

    let isCalcEntries (x: Msg) =
        match x with
        | CalcEntries _ -> true
        | _ -> false

    let isSaveEntries (x: Msg) =
        match x with
        | SaveEntries _ -> true
        | _ -> false

    let isCheckLoadingState (x: Msg) =
        match x with
        | CheckLoadingState -> true
        | _ -> false

