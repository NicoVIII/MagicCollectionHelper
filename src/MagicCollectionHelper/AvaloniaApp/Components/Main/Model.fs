namespace MagicCollectionHelper.AvaloniaApp.Components.Main

open MagicCollectionHelper.Core.Types

type State = { cards: CardEntry list }

type Msg = | ImportFromFile

module Model =
    let init = { cards = [] }
