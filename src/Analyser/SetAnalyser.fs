namespace MagicCollectionHelper

open MagicCollectionHelper.Types

module SetAnalyser =
    type Result = Map<MagicSet, (Set<uint> * uint)>

    type CollectType = Map<MagicSet, Set<uint>>

    let private createEmpty (): CollectType = Map.empty

    let private collect (data: CollectType) (entry: CardEntry): CollectType =
        // We skip cards without set
        match entry.set, entry.number with
        | Some mtgSet, Some number ->
            let set =
                data
                |> Map.tryFind mtgSet
                |> Option.defaultValue Set.empty
                |> Set.add number

            data |> Map.add mtgSet set
        | _ -> data

    let private postprocess (data: CollectType): Result =
        data
        |> Map.map (fun _ set -> (set, Set.count set |> uint))

    let get =
        Analyser.create createEmpty collect postprocess
