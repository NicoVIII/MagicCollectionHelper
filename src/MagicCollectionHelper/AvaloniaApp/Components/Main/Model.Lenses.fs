namespace MagicCollectionHelper.AvaloniaApp.Components.Main

open Myriad.Plugins
open System

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp.Components.Main.Generated

module StateLenses =
    let analyseText =
        StateLenses.common
        << CommonStateLenses.analyseText

    let entries =
        StateLenses.common << CommonStateLenses.entries

    let infoMap =
        StateLenses.common << CommonStateLenses.infoMap

    let prefs =
        StateLenses.common << CommonStateLenses.prefs

    let setData =
        StateLenses.common << CommonStateLenses.setData

    let viewMode =
        StateLenses.common << CommonStateLenses.viewMode
