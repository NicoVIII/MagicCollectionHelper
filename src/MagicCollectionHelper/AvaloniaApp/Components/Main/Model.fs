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
      infoMap: MagicCollectionHelper.Core.Types.CardInfoMap
      prefs: MagicCollectionHelper.Core.Types.Prefs
      setData: MagicCollectionHelper.Core.Types.SetDataMap
      viewMode: ViewMode }

[<Generator.Lenses("components-main", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { common: CommonState
      collection: MagicCollectionHelper.AvaloniaApp.Components.Collection.State
      inventory: MagicCollectionHelper.AvaloniaApp.Components.Inventory.State }

open MagicCollectionHelper.Core.Types
open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components

type Msg =
    | ImportCardInfo
    | SaveCardInfo of CardInfoMap option
    | Analyse
    | ChangeViewMode of ViewMode
    | ChangePrefs of Prefs
    | InventoryMsg of Inventory.Msg
    | CollectionMsg of Collection.Msg

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

    let initCommon () : CommonState =
        { analyseText = arrow
          entries = []
          infoMap = Map.empty
          prefs = Prefs.create false Config.missingPercentDefault false
          setData = CardData.createSetData ()
          viewMode = Collection }

    let init () =
        let state : State =
            { common = initCommon ()
              collection = Components.Collection.Model.init ()
              inventory = Inventory.Model.init () }

        let cmd = Cmd.ofMsg ImportCardInfo

        state, cmd
