namespace MagicCollectionHelper.AvaloniaApp

[<AutoOpen>]
module DomainTypes =
    type Search = { text: string; old: bool option }
