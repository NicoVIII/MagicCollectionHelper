namespace MagicCollectionHelper.AvaloniaApp.Main.Ready

open SimpleOptics

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp.Main.Ready.Generated

module StateLenses =
    let analyseText =
        Optic.compose StateLenses.common CommonStateLenses.analyseText

    let cardInfo = Optic.compose StateLenses.common CommonStateLenses.cardInfo

    let collection = StateLenses.collection

    let dsEntries =
        Optic.compose StateLenses.common CommonStateLenses.dsEntries

    let entries = Optic.compose StateLenses.common CommonStateLenses.entries

    let prefs = Optic.compose StateLenses.common CommonStateLenses.prefs

    let setData = Optic.compose StateLenses.common CommonStateLenses.setData

    let inventory = StateLenses.inventory

    let viewMode = Optic.compose StateLenses.common CommonStateLenses.viewMode
