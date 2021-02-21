namespace MagicCollectionHelper.AvaloniaApp.Components.Main

open Myriad.Plugins
open System

type ViewMode =
    | Analyse
    | Collection
    | Inventory
    | Preferences

[<Generator.Lenses("components-main", "MagicCollectionHelper.Core.Types.Lens")>]
type CommonState =
    { analyseText: string
      entries: MagicCollectionHelper.Core.Types.CardEntry list
      prefs: MagicCollectionHelper.Core.Types.Prefs
      setData: MagicCollectionHelper.Core.Types.SetDataMap
      viewMode: ViewMode }

[<Generator.Lenses("components-main", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    {
        common: CommonState
        inventory: MagicCollectionHelper.AvaloniaApp.Components.Inventory.State
    }

open MagicCollectionHelper.Core.Types
open MagicCollectionHelper.AvaloniaApp.Components

type Msg =
    | ImportCollection
    | Analyse
    | ChangeViewMode of ViewMode
    | ChangePrefs of Prefs
    | InventoryMsg of Inventory.Msg

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

    let initCommon:CommonState =
        { analyseText = arrow
          entries = []
          prefs = Prefs.create false Config.missingPercentDefault false
          setData = CardData.createSetData ()
          viewMode = Collection }

    let init: State =
        { common = initCommon
          inventory = Inventory.Model.init }
