namespace MagicCollectionHelper.AvaloniaApp.Components.Main

open Myriad.Plugins

type ViewMode =
    | Analyse
    | Config
    | Start

[<Generator.Lenses("components-main", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { analyseText: string
      cards: MagicCollectionHelper.Core.Types.CardEntry list
      setData: MagicCollectionHelper.Core.Types.SetDataMap
      viewMode: ViewMode }

type Msg =
    | Import
    | Analyse

type Dispatch = Msg -> unit

module Model =
    open MagicCollectionHelper.Core

    let init =
        { analyseText = ""
          cards = []
          setData = CardData.createSetData ()
          viewMode = Start }
