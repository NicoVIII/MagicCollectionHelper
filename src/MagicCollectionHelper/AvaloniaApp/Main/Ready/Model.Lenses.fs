namespace MagicCollectionHelper.AvaloniaApp.Main.Ready

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp.Main.Ready.Generated

module StateLenses =
    let analyseText =
        StateLenses.common
        << CommonStateLenses.analyseText

    let cardInfo =
        StateLenses.common << CommonStateLenses.cardInfo

    let collection = StateLenses.collection

    let dsEntries =
        StateLenses.common << CommonStateLenses.dsEntries

    let entries =
        StateLenses.common << CommonStateLenses.entries

    let prefs =
        StateLenses.common << CommonStateLenses.prefs

    let setData =
        StateLenses.common << CommonStateLenses.setData

    let inventory = StateLenses.inventory

    let viewMode =
        StateLenses.common << CommonStateLenses.viewMode
