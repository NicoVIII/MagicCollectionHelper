namespace MagicCollectionHelper.AvaloniaApp

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp

[<AutoOpen>]
module DomainTypeModules =
    module Search =
        let fits (search: Search) oldableEntry =
            let entry = oldableEntry.data.entry
            let info = oldableEntry.data.info

            let fitOld () =
                match search.old with
                | None -> true
                | Some old -> (oldableEntry.amountOld = entry.amount) = old

            let fitText () = String.iContains info.name search.text

            fitOld () && fitText ()
