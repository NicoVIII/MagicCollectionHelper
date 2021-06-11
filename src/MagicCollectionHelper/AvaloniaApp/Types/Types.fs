namespace MagicCollectionHelper.AvaloniaApp

open Myriad.Plugins

[<AutoOpen>]
module DomainTypes =
    [<Generator.Lenses("types", "MagicCollectionHelper.Core.Types.Lens")>]
    [<Generator.Fields("types")>]
    type Search = { text: string; old: bool option }
