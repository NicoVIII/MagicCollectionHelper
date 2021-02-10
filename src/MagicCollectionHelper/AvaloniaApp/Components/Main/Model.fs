namespace MagicCollectionHelper.AvaloniaApp.Components.Main

open Myriad.Plugins

open MagicCollectionHelper.AvaloniaApp

[<Generator.Lenses("components-main", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { cards: MagicCollectionHelper.Core.Types.CardEntry list
      setData: MagicCollectionHelper.Core.Types.SetDataMap
      text: string }

type Msg =
    | ImportFromFile
    | Analyse

type Dispatch = Msg -> unit

module Model =
    open MagicCollectionHelper.Core

    let init =
        { cards = []
          setData = CardData.createSetData ()
          text = Config.startText }
