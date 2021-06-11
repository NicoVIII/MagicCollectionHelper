namespace MagicCollectionHelper.AvaloniaApp

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp

[<AutoOpen>]
module DomainTypeModules =
    module Search =
        let fits (search: Search) (agedEntryWithInfo: AgedCardEntryWithInfo) =
            let entry = agedEntryWithInfo.data.data
            let amountOld = agedEntryWithInfo.data.amountOld
            let info = agedEntryWithInfo.info

            let fitOld () =
                match search.old with
                | None -> true
                | Some old -> (amountOld = entry.amount) = old

            let fitText () = String.iContains info.name search.text

            fitOld () && fitText ()
