namespace MagicCollectionHelper.AvaloniaApp.Main

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Main.Generated

module StateLenses =
    let analyseText =
        StateLenses.common
        << CommonStateLenses.analyseText

    let cardInfo =
        StateLenses.common
        << CommonStateLenses.cardInfo
        << LoadableLenses.data

    let cardInfoState =
        StateLenses.common
        << CommonStateLenses.cardInfo
        << LoadableLenses.state

    let dsEntries =
        StateLenses.common
        << CommonStateLenses.dsEntries
        << LoadableLenses.data

    let dsEntriesState =
        StateLenses.common
        << CommonStateLenses.dsEntries
        << LoadableLenses.state

    let entries =
        StateLenses.common
        << CommonStateLenses.entries
        << LoadableLenses.data

    let entriesState =
        StateLenses.common
        << CommonStateLenses.entries
        << LoadableLenses.state

    let prefs =
        StateLenses.common << CommonStateLenses.prefs

    let setData =
        StateLenses.common
        << CommonStateLenses.setData
        << LoadableLenses.data

    let setDataState =
        StateLenses.common
        << CommonStateLenses.setData
        << LoadableLenses.state

    let viewMode =
        StateLenses.common << CommonStateLenses.viewMode
