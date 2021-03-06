namespace MagicCollectionHelper.Core.Persistence

open TypedPersistence.Json

open MagicCollectionHelper.Core

module CustomLocation =
    let fileName = "locations.json"

    let filePath = [ SystemInfo.savePath; fileName ] |> Path.combine

    let save = save<CustomLocation list> filePath

    let load () = load<CustomLocation list> filePath
