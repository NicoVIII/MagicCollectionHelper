namespace MagicCollectionHelper.AvaloniaApp.Main.Ready

open Myriad.Plugins
open System

type ViewMode =
    | Analyse
    | Collection
    | Inventory
    | Preferences

[<Generator.Lenses("main.ready", "MagicCollectionHelper.Core.Types.Lens")>]
type CommonState =
    { analyseText: string
      cardInfo: MagicCollectionHelper.Core.Types.CardInfoMap
      dsEntries: MagicCollectionHelper.Core.Types.DeckStatsCardEntry list
      entries: MagicCollectionHelper.Core.Types.OldAmountable<MagicCollectionHelper.Core.Types.CardEntry> list
      prefs: MagicCollectionHelper.Core.Types.Prefs
      setData: MagicCollectionHelper.Core.Types.SetDataMap
      viewMode: ViewMode }

[<Generator.Lenses("main.ready", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { common: CommonState
      collection: MagicCollectionHelper.AvaloniaApp.Components.Collection.State
      inventory: MagicCollectionHelper.AvaloniaApp.Components.Inventory.State }

open MagicCollectionHelper.Core.Types
open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components

[<Generator.DuCases("main.ready")>]
type Msg =
    | AsyncError of exn
    | Analyse
    | ChangeViewMode of ViewMode
    | ChangePrefs of Prefs
    | SavePrefs of Prefs
    | SaveEntries of CardEntry list
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

    let initCommon cardInfo dsEntries entries setData =
        // If we init entries, every entry is old
        let entries =
            List.map
                (fun entry ->
                    { amountOld = entry.amount
                      data = entry })
                entries

        let prefs =
            Persistence.Prefs.load ()
            |> Option.defaultValue (Prefs.create false Config.missingPercentDefault false)

        let state =
            { analyseText = arrow
              cardInfo = cardInfo
              dsEntries = dsEntries
              entries = entries
              prefs = prefs
              setData = setData
              viewMode = Collection }

        state

    let init cardInfo dsEntries entries setData =
        { common = initCommon cardInfo dsEntries entries setData
          collection = Collection.Model.init ()
          inventory = Inventory.Model.init () }
