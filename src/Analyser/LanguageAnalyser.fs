namespace MagicCollectionHelper

open MagicCollectionHelper.Types

module LanguageAnalyser =
    type Result = Map<Language, uint>

    type CollectData = Map<Language, uint>

    type Settings = unit

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

    let private postprocess (_: SetDataMap) (data: CollectData): Result = data

    let print (_: Settings) (result: Result) =
        let p = sprintf

        let title = seq { "Language Analysis" }

        let data =
            result
            |> Map.map (fun (Language key) value -> p "%s: %i" key value)
            |> Map.toSeq
            |> Seq.map (fun (_, value) -> value)

        [ title; data ] |> Seq.concat

    let get =
        Analyser.create createEmpty collect postprocess print
