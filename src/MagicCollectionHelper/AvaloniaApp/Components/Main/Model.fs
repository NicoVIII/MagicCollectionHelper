namespace MagicCollectionHelper.AvaloniaApp.Components.Main

open Myriad.Plugins

[<Generator.Lenses("components-main", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { cards: MagicCollectionHelper.Core.Types.CardEntry list }

type Msg = | ImportFromFile

module Model =
    let init = { cards = [] }
