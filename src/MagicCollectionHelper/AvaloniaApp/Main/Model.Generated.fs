//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.AvaloniaApp.Main.Generated

module CommonStateLenses =
    open MagicCollectionHelper.AvaloniaApp.Main
    let analyseText =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.analyseText),
            (fun (x: CommonState) (value: string) -> { x with analyseText = value })
        )

    let cardInfo =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.cardInfo),
            (fun (x: CommonState) (value: MagicCollectionHelper.AvaloniaApp.Loadable<MagicCollectionHelper.Core.Types.CardInfoMap>) ->
                { x with cardInfo = value })
        )

    let dsEntries =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.dsEntries),
            (fun (x: CommonState) (value: MagicCollectionHelper.AvaloniaApp.Loadable<MagicCollectionHelper.Core.Types.DeckStatsCardEntry list>) ->
                { x with dsEntries = value })
        )

    let entries =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.entries),
            (fun (x: CommonState) (value: MagicCollectionHelper.AvaloniaApp.Loadable<MagicCollectionHelper.Core.Types.CardEntry list>) ->
                { x with entries = value })
        )

    let prefs =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.prefs),
            (fun (x: CommonState) (value: MagicCollectionHelper.Core.Types.Prefs) -> { x with prefs = value })
        )

    let setData =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.setData),
            (fun (x: CommonState) (value: MagicCollectionHelper.AvaloniaApp.Loadable<MagicCollectionHelper.Core.Types.SetDataMap>) ->
                { x with setData = value })
        )

    let viewMode =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.viewMode),
            (fun (x: CommonState) (value: ViewMode) -> { x with viewMode = value })
        )
namespace rec MagicCollectionHelper.AvaloniaApp.Main.Generated

module StateLenses =
    open MagicCollectionHelper.AvaloniaApp.Main
    let common =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: State) -> x.common),
            (fun (x: State) (value: CommonState) -> { x with common = value })
        )

    let collection =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: State) -> x.collection),
            (fun (x: State) (value: MagicCollectionHelper.AvaloniaApp.Components.Collection.State) ->
                { x with collection = value })
        )

    let inventory =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: State) -> x.inventory),
            (fun (x: State) (value: MagicCollectionHelper.AvaloniaApp.Components.Inventory.State) ->
                { x with inventory = value })
        )
namespace rec MagicCollectionHelper.AvaloniaApp.Main.Generated

module Msg =
    open MagicCollectionHelper.AvaloniaApp.Main
    let toString (x: Msg) =
        match x with
        | AsyncError _ -> "AsyncError"
        | StartUp -> "StartUp"
        | PrepareCardInfo -> "PrepareCardInfo"
        | ImportCardInfo _ -> "ImportCardInfo"
        | SaveCardInfo _ -> "SaveCardInfo"
        | PrepareSetData -> "PrepareSetData"
        | ImportSetData _ -> "ImportSetData"
        | SaveSetData _ -> "SaveSetData"
        | CheckLoadingState -> "CheckLoadingState"
        | CalcEntries -> "CalcEntries"
        | SaveEntries _ -> "SaveEntries"
        | Analyse -> "Analyse"
        | ChangeViewMode _ -> "ChangeViewMode"
        | ChangePrefs _ -> "ChangePrefs"
        | SavePrefs _ -> "SavePrefs"
        | InventoryMsg _ -> "InventoryMsg"
        | CollectionMsg _ -> "CollectionMsg"

    let fromString (x: string) =
        match x with
        | "StartUp" -> Some StartUp
        | "PrepareCardInfo" -> Some PrepareCardInfo
        | "PrepareSetData" -> Some PrepareSetData
        | "CheckLoadingState" -> Some CheckLoadingState
        | "CalcEntries" -> Some CalcEntries
        | "Analyse" -> Some Analyse
        | _ -> None

    let toTag (x: Msg) =
        match x with
        | AsyncError _ -> 0
        | StartUp -> 1
        | PrepareCardInfo -> 2
        | ImportCardInfo _ -> 3
        | SaveCardInfo _ -> 4
        | PrepareSetData -> 5
        | ImportSetData _ -> 6
        | SaveSetData _ -> 7
        | CheckLoadingState -> 8
        | CalcEntries -> 9
        | SaveEntries _ -> 10
        | Analyse -> 11
        | ChangeViewMode _ -> 12
        | ChangePrefs _ -> 13
        | SavePrefs _ -> 14
        | InventoryMsg _ -> 15
        | CollectionMsg _ -> 16

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

    let isCheckLoadingState (x: Msg) =
        match x with
        | CheckLoadingState -> true
        | _ -> false

    let isCalcEntries (x: Msg) =
        match x with
        | CalcEntries -> true
        | _ -> false

    let isSaveEntries (x: Msg) =
        match x with
        | SaveEntries _ -> true
        | _ -> false

    let isAnalyse (x: Msg) =
        match x with
        | Analyse -> true
        | _ -> false

    let isChangeViewMode (x: Msg) =
        match x with
        | ChangeViewMode _ -> true
        | _ -> false

    let isChangePrefs (x: Msg) =
        match x with
        | ChangePrefs _ -> true
        | _ -> false

    let isSavePrefs (x: Msg) =
        match x with
        | SavePrefs _ -> true
        | _ -> false

    let isInventoryMsg (x: Msg) =
        match x with
        | InventoryMsg _ -> true
        | _ -> false

    let isCollectionMsg (x: Msg) =
        match x with
        | CollectionMsg _ -> true
        | _ -> false
