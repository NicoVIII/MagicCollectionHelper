namespace MagicCollectionHelper.AvaloniaApp.Main.Ready

open System

type ViewMode =
    | Analyse
    | Collection
    | Inventory
    | Preferences

type CommonState =
    {
        analyseText: string
        cardInfo: MagicCollectionHelper.Core.CardTypes.CardInfoMap
        dsEntries: MagicCollectionHelper.Core.DomainTypes.DeckStatsCardEntry list
        entries: MagicCollectionHelper.Core.CardTypes.OldAmountable<MagicCollectionHelper.Core.CardTypes.Entry> list
        prefs: MagicCollectionHelper.Core.DomainTypes.Prefs
        setData: MagicCollectionHelper.Core.DomainTypes.SetDataMap
        viewMode: ViewMode
    }

type State =
    {
        common: CommonState
        collection: MagicCollectionHelper.AvaloniaApp.Components.Collection.State
        inventory: MagicCollectionHelper.AvaloniaApp.Components.Inventory.State
    }

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components

type Msg =
    | AsyncError of exn
    | Analyse
    | ChangeViewMode of ViewMode
    | ChangePrefs of (Prefs -> Prefs)
    | SavePrefs
    | SaveEntries of Entry list
    | InventoryMsg of Inventory.Msg
    | CollectionMsg of Collection.Msg

type Dispatch = Msg -> unit

module Model =
    let arrow =
        [
            "     ."
            "   .:;:."
            " .:;;;;;:."
            "   ;;;;;"
            "   ;;;;; Press here after 'Import'"
            "   ;;;;;"
        ]
        |> String.concat Environment.NewLine

    let initCommon cardInfo dsEntries entries setData =
        // If we init entries, every entry is old
        let entries =
            List.map
                (fun entry ->
                    {
                        amountOld = entry.amount
                        data = entry
                    })
                entries

        let prefs =
            Persistence.Prefs.load ()
            |> Option.defaultValue (Prefs.create 20u 40u Decimal Config.missingPercentDefault false)

        let state =
            {
                analyseText = arrow
                cardInfo = cardInfo
                dsEntries = dsEntries
                entries = entries
                prefs = prefs
                setData = setData
                viewMode = Collection
            }

        state

    let init cardInfo dsEntries entries setData =
        {
            common = initCommon cardInfo dsEntries entries setData
            collection = Collection.Model.init ()
            inventory = Inventory.Model.init ()
        }
