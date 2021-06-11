namespace MagicCollectionHelper.Core.Persistence

open TypedPersistence.Json

open MagicCollectionHelper.Core

module Prefs =
    let fileName = "settings.json"

    let filePath =
        [ SystemInfo.savePath; fileName ] |> Path.combine

    let save = save<Prefs> filePath

    let load () = load<Prefs> filePath
