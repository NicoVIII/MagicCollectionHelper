namespace MagicCollectionHelper

open MagicCollectionHelper.Types

module BasicAnalyser =
    type Result =
        { amount: uint
          uniqueWithSet: uint
          withSet: uint
          foils: uint }

    type CollectData =
        { amount: uint
          foils: uint
          uniqueWithSet: Set<MagicSet * uint>
          withSet: uint }

    let private createEmpty (): CollectData =
        { amount = 0u
          uniqueWithSet = Set.empty<MagicSet * uint>
          foils = 0u
          withSet = 0u }

    let private collect (data: CollectData) (entry: CardEntry) =
        let addFoils =
            match entry.foil with
            | true -> entry.amount
            | false -> 0u

        let unique, withSet =
            match entry.set, entry.number with
            | Some set, Some number ->
                let unique =
                    data.uniqueWithSet |> Set.add (set, number)

                (unique, data.withSet + 1u)
            | _ -> (data.uniqueWithSet, data.withSet)

        { data with
              amount = data.amount + entry.amount
              uniqueWithSet = unique
              foils = data.foils + addFoils
              withSet = withSet }

    let postprocess data: Result =
        { amount = data.amount
          uniqueWithSet = data.uniqueWithSet |> Set.count |> uint
          withSet = data.withSet
          foils = data.foils }

    let get =
        Analyser.create createEmpty collect postprocess
