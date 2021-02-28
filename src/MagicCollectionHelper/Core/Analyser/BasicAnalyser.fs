namespace MagicCollectionHelper.Core

open MagicCollectionHelper.Core.Types

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

    type Preferences = { dozenalize: bool }

    let private createEmpty (): CollectData =
        { amount = 0u
          uniqueWithSet = Set.empty
          foils = 0u
          withSet = 0u
          withLanguage = 0u }

    let private collect _ (data: CollectData) (entry: DeckStatsCardEntry) =
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

    let print (settings: Preferences) (result: Result) =
        let p = sprintf

        let inline pN x =
            Numbers.print settings.dozenalize 0uy (int x)

        seq {
            "Basic Analysis"
            p "%5s - Amount" (pN result.amount)
            p "%5s - With Language" (pN result.withLanguage)
            p "%5s - With Set" (pN result.withSet)
            p "%5s - With Set (unique)" (pN result.uniqueWithSet)
            p "%5s - Foils" (pN result.foils)
        }

    let get =
        Analyser.create createEmpty collect postprocess print
