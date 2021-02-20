namespace MagicCollectionHelper.AvaloniaApp.Components.Main

open Myriad.Plugins
open System

open MagicCollectionHelper.Core.Types

type ViewMode =
    | Analyse
    | Collection
    | Preferences

[<Generator.Lenses("components-main", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { analyseText: string
      cards: MagicCollectionHelper.Core.Types.CardEntry list
      prefs: MagicCollectionHelper.Core.Types.Prefs
      setData: MagicCollectionHelper.Core.Types.SetDataMap
      viewMode: ViewMode }

type Msg =
    | ImportCollection
    | Analyse
    | ChangeViewMode of ViewMode
    | ChangePrefs of Prefs

type Dispatch = Msg -> unit

module Model =
    open MagicCollectionHelper.Core

    let arrow =
        [ "     ."
          "   .:;:."
          " .:;;;;;:."
          "   ;;;;;"
          "   ;;;;; Press here after 'Import'"
          "   ;;;;;" ]
        |> String.concat Environment.NewLine

    let init =
        { analyseText = arrow
          cards = []
          prefs = Types.Prefs.create false Config.missingPercentDefault false
          setData = CardData.createSetData ()
          viewMode = Collection }
