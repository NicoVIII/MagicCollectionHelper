namespace MagicCollectionHelper.AvaloniaApp

open SimpleOptics

[<AutoOpen>]
module DomainOptics =
    [<RequireQualifiedAccess>]
    module SearchOptic =
        let text =
            Lens((fun (search: Search) -> search.text), (fun search value -> { search with text = value }))

        let old =
            Lens((fun (search: Search) -> search.old), (fun search value -> { search with old = value }))
