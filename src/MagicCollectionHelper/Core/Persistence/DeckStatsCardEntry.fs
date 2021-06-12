namespace MagicCollectionHelper.Core.Persistence

open TypedPersistence.Json

open MagicCollectionHelper.Core

module DeckStatsCardEntry =
    let fileName = "entries.json"

    let filePath = [ SystemInfo.savePath; fileName ] |> Path.combine

    let save = save<DeckStatsCardEntry list> filePath

    let load () = load<DeckStatsCardEntry list> filePath

    let loadAsync () = async { return load () }
