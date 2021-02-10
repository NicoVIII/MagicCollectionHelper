namespace MagicCollectionHelper.AvaloniaApp.Components.Main

open Myriad.Plugins

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
          text = "This is a placeholder text! This is the first, very basic version of a GUI." }
