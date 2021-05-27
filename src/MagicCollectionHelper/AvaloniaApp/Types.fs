namespace MagicCollectionHelper.AvaloniaApp

open Myriad.Plugins

open MagicCollectionHelper.Core.Types

[<Generator.DuCases("root")>]
type LoadingState =
    | Prepare
    | Download
    | Import
    | Ready

type Loadable<'a> = { data: 'a; state: LoadingState }

module Loadable =
    let create data = { data = data; state = Prepare }

module LoadableLenses =
    let data =
        Lens((fun (x: Loadable<'a>) -> x.data), (fun (x: Loadable<'a>) (value: 'a) -> { x with data = value }))

    let state =
        Lens(
            (fun (x: Loadable<'a>) -> x.state),
            (fun (x: Loadable<'a>) (value: LoadingState) -> { x with state = value })
        )
