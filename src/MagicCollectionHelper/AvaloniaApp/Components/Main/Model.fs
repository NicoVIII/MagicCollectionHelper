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
      entries: MagicCollectionHelper.Core.Types.DeckStatsCardEntry list
      infoMap: Map<MagicCollectionHelper.Core.Types.MagicSet * MagicCollectionHelper.Core.Types.SetNumber, MagicCollectionHelper.Core.Types.CardInfo>
      loadInProgress: bool
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
    | ImportCardInfo
    | ImportCollection
    | SaveCardInfo of Map<MagicSet * SetNumber,CardInfo> option
    | SaveCollection of DeckStatsCardEntry seq option
    | Analyse
    | ChangeViewMode of ViewMode
    | ChangePrefs of Prefs
    | InventoryMsg of Inventory.Msg

type Dispatch = Msg -> unit

module Model =
    open Elmish

    open MagicCollectionHelper.Core

    let arrow =
        [ "     ."
          "   .:;:."
          " .:;;;;;:."
          "   ;;;;;"
          "   ;;;;; Press here after 'Import'"
          "   ;;;;;" ]
        |> String.concat Environment.NewLine

    let initCommon (): CommonState =
        { analyseText = arrow
          entries = []
          infoMap = Map.empty
          loadInProgress = false
          prefs = Prefs.create false Config.missingPercentDefault false
          setData = CardData.createSetData ()
          viewMode = Collection }

    let init () =
        let state: State =
            { common = initCommon ()
              inventory = Inventory.Model.init () }

        let cmd = Cmd.ofMsg ImportCardInfo

        state, cmd
