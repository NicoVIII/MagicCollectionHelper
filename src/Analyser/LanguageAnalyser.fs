namespace MagicCollectionHelper

open MagicCollectionHelper.Types

module LanguageAnalyser =
    type Result = Map<Language, uint>

    type CollectData = Map<Language, uint>

    let private createEmpty (): Result = Map.empty

    let private collect (data: CollectData) (entry: CardEntry): Result =
        // We skip cards without language
        match entry.language with
        | Some language ->
            let amount =
                data
                |> Map.tryFind language
                |> Option.defaultValue 0u
                |> (+) 1u

            data |> Map.add language amount
        | None -> data

    let private postprocess (data: CollectData): Result = id data

    let get =
        Analyser.create createEmpty collect postprocess
