namespace MagicCollectionHelper.AvaloniaApp.Main.Loading

open Myriad.Plugins

type Loadable<'a> =
    | Prepare
    | Download
    | Import
    | Ready of 'a

[<Generator.Lenses("main.loading", "SimpleOptics.Lens")>]
type State =
    { cardInfo: Loadable<MagicCollectionHelper.Core.CardTypes.CardInfoMap>
      dsEntries: Loadable<MagicCollectionHelper.Core.DomainTypes.DeckStatsCardEntry list>
      entries: Loadable<MagicCollectionHelper.Core.CardTypes.Entry list>
      setData: Loadable<MagicCollectionHelper.Core.DomainTypes.SetDataMap> }

open Elmish

open MagicCollectionHelper.Core

[<Generator.DuCases("main.loading")>]
type Msg =
    | AsyncError of exn
    | StartUp
    | PrepareCardInfo
    | ImportCardInfo of string
    | SaveCardInfo of CardInfoMap
    | LoadCollection
    | SaveCollection of DeckStatsCardEntry list option
    | PrepareSetData
    | ImportSetData of string
    | SaveSetData of SetDataMap
    | CalcEntries of CardInfoMap * DeckStatsCardEntry list
    | SaveEntries of Entry list
    | CheckLoadingState

type Dispatch = Msg -> unit

type Intent =
    | DoNothing
    | ChangeToReady of CardInfoMap * DeckStatsCardEntry list * Entry list * SetDataMap

module Model =
    let init () =
        let state =
            { cardInfo = Prepare
              dsEntries = Prepare
              entries = Prepare
              setData = Prepare }

        state, Cmd.ofMsg StartUp
