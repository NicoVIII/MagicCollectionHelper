namespace MagicCollectionHelper.AvaloniaApp.Components.Main

open Myriad.Plugins
open System

type ViewMode =
    | Analyse
    | Collection
    | Inventory
    | Preferences

type LocationCardMap =
    Map<MagicCollectionHelper.Core.Types.InventoryLocation, MagicCollectionHelper.Core.Types.CardEntry list>

[<Generator.Lenses("components-main", "MagicCollectionHelper.Core.Types.Lens")>]
type State =
    { analyseText: string
      inventory: LocationCardMap
      cards: MagicCollectionHelper.Core.Types.CardEntry list
      locations: MagicCollectionHelper.Core.Types.CustomLocation list
      prefs: MagicCollectionHelper.Core.Types.Prefs
      setData: MagicCollectionHelper.Core.Types.SetDataMap
      viewMode: ViewMode }

open MagicCollectionHelper.Core.Types

type Msg =
    | ImportCollection
    | Analyse
    | ChangeViewMode of ViewMode
    | ChangePrefs of Prefs
    | TakeInventory

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
        // Test Locations
        let locations = [
            { name = "Collection GRN"
              rules = [ InSet [MagicSet "GRN"]; Limit 1u ] }
            { name = "Collection RNA"
              rules = [ InSet [MagicSet "RNA"]; Limit 1u ] }
            { name = "Collection WAR"
              rules = [ InSet [MagicSet "WAR"]; Limit 1u ] }
            { name = "Collection ELD"
              rules = [ InSet [MagicSet "ELD"]; Limit 1u ] }
            { name = "Collection THB"
              rules = [ InSet [MagicSet "THB"]; Limit 1u ] }
            { name = "Collection IKO"
              rules = [ InSet [MagicSet "IKO"]; Limit 1u ] }
            { name = "Collection ZNR"
              rules = [ InSet [MagicSet "ZNR"]; Limit 1u ] }
            { name = "Collection KHM"
              rules = [ InSet [MagicSet "KHM"]; Limit 1u ] }
            { name = "Lookup"
              rules = [ Limit 1u ] }
        ]

        { analyseText = arrow
          inventory = Map.empty
          cards = []
          locations = locations
          prefs = Prefs.create false Config.missingPercentDefault false
          setData = CardData.createSetData ()
          viewMode = Collection }
