namespace MagicCollectionHelper

open MagicCollectionHelper.Types

module LanguageAnalyser =
    type Result = Map<Language, uint>

    type CollectData = Map<Language, uint>

    type Settings = { dozenalize: bool }

    let private createEmpty (): Result = Map.empty

    let private collect _ (data: CollectData) (entry: CardEntry): Result =
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

    let print (settings: Settings) (result: Result) =
        let p = sprintf

        let title = seq { "Language Analysis" }

        let data =
            result
            |> Map.map (fun (Language key) value -> $"%s{key}: %s{Numbers.print settings.dozenalize 0uy (int value)}")
            |> Map.toSeq
            |> Seq.map (fun (_, value) -> value)

        [ title; data ] |> Seq.concat

    let get =
        Analyser.create createEmpty collect postprocess print
