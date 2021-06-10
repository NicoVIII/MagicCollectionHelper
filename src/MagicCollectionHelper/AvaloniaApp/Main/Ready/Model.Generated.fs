//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.AvaloniaApp.Main.Ready.Generated

module CommonStateLenses =
    open MagicCollectionHelper.AvaloniaApp.Main.Ready
    let analyseText =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.analyseText),
            (fun (x: CommonState) (value: string) -> { x with analyseText = value })
        )

    let cardInfo =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.cardInfo),
            (fun (x: CommonState) (value: MagicCollectionHelper.Core.Types.CardInfoMap) -> { x with cardInfo = value })
        )

    let dsEntries =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.dsEntries),
            (fun (x: CommonState) (value: MagicCollectionHelper.Core.Types.DeckStatsCardEntry list) ->
                { x with dsEntries = value })
        )

    let entries =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.entries),
            (fun (x: CommonState) (value: MagicCollectionHelper.Core.Types.OldAmountable<MagicCollectionHelper.Core.Types.CardEntry> list) ->
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
            (fun (x: CommonState) (value: MagicCollectionHelper.Core.Types.SetDataMap) -> { x with setData = value })
        )

    let viewMode =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: CommonState) -> x.viewMode),
            (fun (x: CommonState) (value: ViewMode) -> { x with viewMode = value })
        )
namespace rec MagicCollectionHelper.AvaloniaApp.Main.Ready.Generated

module StateLenses =
    open MagicCollectionHelper.AvaloniaApp.Main.Ready
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
namespace rec MagicCollectionHelper.AvaloniaApp.Main.Ready.Generated

module Msg =
    open MagicCollectionHelper.AvaloniaApp.Main.Ready
    let toString (x: Msg) =
        match x with
        | AsyncError _ -> "AsyncError"
        | Analyse -> "Analyse"
        | ChangeViewMode _ -> "ChangeViewMode"
        | ChangePrefs _ -> "ChangePrefs"
        | SavePrefs _ -> "SavePrefs"
        | SaveEntries _ -> "SaveEntries"
        | InventoryMsg _ -> "InventoryMsg"
        | CollectionMsg _ -> "CollectionMsg"

    let fromString (x: string) =
        match x with
        | "Analyse" -> Some Analyse
        | _ -> None

    let toTag (x: Msg) =
        match x with
        | AsyncError _ -> 0
        | Analyse -> 1
        | ChangeViewMode _ -> 2
        | ChangePrefs _ -> 3
        | SavePrefs _ -> 4
        | SaveEntries _ -> 5
        | InventoryMsg _ -> 6
        | CollectionMsg _ -> 7

    let isAsyncError (x: Msg) =
        match x with
        | AsyncError _ -> true
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

    let isSaveEntries (x: Msg) =
        match x with
        | SaveEntries _ -> true
        | _ -> false

    let isInventoryMsg (x: Msg) =
        match x with
        | InventoryMsg _ -> true
        | _ -> false

    let isCollectionMsg (x: Msg) =
        match x with
        | CollectionMsg _ -> true
        | _ -> false
