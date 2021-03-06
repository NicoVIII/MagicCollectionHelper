namespace MagicCollectionHelper.Core

[<RequireQualifiedAccess>]
module Config =
    /// Name of the used subfolder for data of this application
    let dataFolderName = "magic-collection-helper"
    let missingPercentDefault = 0.8
    let maxAgeCardDataHours = 24. * 7.
