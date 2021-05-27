namespace MagicCollectionHelper.AvaloniaApp.Main

open Myriad.Plugins
open System

type ViewMode =
    | Loading
    | Analyse
    | Collection
    | Inventory
    | Preferences

[<Generator.Lenses("main", "MagicCollectionHelper.Core.Types.Lens")>]
type CommonState =
    { analyseText: string
      cardInfo: MagicCollectionHelper.AvaloniaApp.Loadable<MagicCollectionHelper.Core.Types.CardInfoMap>
      dsEntries: MagicCollectionHelper.Core.Types.DeckStatsCardEntry list
      entries: MagicCollectionHelper.Core.Types.CardEntry list
      prefs: MagicCollectionHelper.Core.Types.Prefs
      setData: MagicCollectionHelper.AvaloniaApp.Loadable<MagicCollectionHelper.Core.Types.SetDataMap>
      viewMode: ViewMode }

[<Generator.Lenses("main", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { common: CommonState
      collection: MagicCollectionHelper.AvaloniaApp.Components.Collection.State
      inventory: MagicCollectionHelper.AvaloniaApp.Components.Inventory.State }

open MagicCollectionHelper.Core.Types
open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components

type Msg =
    | AsyncError of exn
    | StartUp
    | PrepareCardInfo
    | ImportCardInfo of string
    | SaveCardInfo of CardInfoMap
    | PrepareSetData
    | ImportSetData of string
    | SaveSetData of SetDataMap
    | CheckLoadingState
    | CalcEntries
    | SaveEntries of CardEntry list
    | Analyse
    | ChangeViewMode of ViewMode
    | ChangePrefs of Prefs
    | SavePrefs of Prefs
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

    let initCommon () : CommonState * Cmd<Msg> =
        let prefs =
            Persistence.Prefs.load ()
            |> Option.defaultValue (Prefs.create false Config.missingPercentDefault false)

        let state =
            { analyseText = arrow
              cardInfo = Map.empty |> Loadable.create
              dsEntries = []
              entries = []
              prefs = prefs
              setData = Map.empty |> Loadable.create
              viewMode = Loading }

        state, Cmd.ofMsg StartUp

    let init () =
        let commonState, commonCmd = initCommon ()

        let state : State =
            { common = commonState
              collection = Collection.Model.init ()
              inventory = Inventory.Model.init () }

        let cmd = [ commonCmd ] |> Cmd.batch

        state, cmd
