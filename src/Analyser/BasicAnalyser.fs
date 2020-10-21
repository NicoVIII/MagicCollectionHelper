namespace MagicCollectionHelper

open MagicCollectionHelper.Types

open System.Text

module BasicAnalyser =
    type Result =
        { amount: uint
          uniqueWithSet: uint
          withLanguage: uint
          withSet: uint
          foils: uint }

    type CollectData =
        { amount: uint
          foils: uint
          uniqueWithSet: Set<MagicSet * SetNumber>
          withSet: uint
          withLanguage: uint }

    type Settings = unit

    let private createEmpty (): CollectData =
        { amount = 0u
          uniqueWithSet = Set.empty
          foils = 0u
          withSet = 0u
          withLanguage = 0u }

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

        let withLanguage =
            match entry.language with
            | Some language -> data.withLanguage + 1u
            | None -> data.withLanguage

        { data with
              amount = data.amount + entry.amount
              uniqueWithSet = unique
              foils = data.foils + addFoils
              withSet = withSet
              withLanguage = withLanguage }

    let postprocess (_: SetDataMap) data: Result =
        { amount = data.amount
          uniqueWithSet = data.uniqueWithSet |> Set.count |> uint
          withSet = data.withSet
          foils = data.foils
          withLanguage = data.withLanguage }

    let print (_: Settings) (result: Result) =
        let p = sprintf

        seq {
            p "Basic Analysis"
            p "%5i - Amount" result.amount
            p "%5i - With Language" result.withLanguage
            p "%5i - With Set" result.withSet
            p "%5i - With Set (unique)" result.uniqueWithSet
            p "%5i - Foils" result.foils
        }

    let get =
        Analyser.create createEmpty collect postprocess print
