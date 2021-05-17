namespace MagicCollectionHelper.Core.Persistence

open TypedPersistence.Json

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

module RawCustomLocation =
    let fileName = "locations.json"

    let filePath =
        [ SystemInfo.savePath; fileName ] |> Path.combine

    let save = save<RawCustomLocation list> filePath

    let load () = load<RawCustomLocation list> filePath
