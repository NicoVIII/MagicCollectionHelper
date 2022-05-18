namespace MagicCollectionHelper.AvaloniaApp

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp

[<AutoOpen>]
module DomainTypeModules =
    module Search =
        let fits (search: Search) (entry: AgedEntryWithInfo) =
            let amount = entry ^. AgedEntryWithInfoOptic.amount

            let amountOld = entry ^. AgedEntryWithInfoOptic.amountOld

            let name = entry ^. AgedEntryWithInfoOptic.name

            let fitOld () =
                match search.old with
                | None -> true
                | Some old -> (amountOld = amount) = old

            let fitText () = String.iContains name search.text

            fitOld () && fitText ()
