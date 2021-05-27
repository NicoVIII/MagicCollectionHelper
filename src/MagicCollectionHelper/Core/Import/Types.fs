namespace MagicCollectionHelper.Core.Import

[<AutoOpen>]
module Types =
    type ImportPreparationResult =
        | FileExists of string
        | DownloadFile of Async<string>
