namespace MagicCollectionHelper.Core

module Analyser =
    /// Combine analysers to use same loop for collection
    let combine analyser1 analyser2 =
        let createEmpty () =
            (analyser2.emptyData (), analyser1.emptyData ())

        let collect (prefs2, prefs1) (data2, data1) entry =
            let result1 = analyser1.collect prefs1 data1 entry
            let result2 = analyser2.collect prefs2 data2 entry
            (result2, result1)

        let postprocess setData (data2, data1) =
            let result1 = analyser1.postprocess setData data1
            let result2 = analyser2.postprocess setData data2
            (result2, result1)

        let print (prefs2, prefs1) (result2, result1) =
            let result1 = analyser1.print prefs1 result1
            let result2 = analyser2.print prefs2 result2
            [ result2; seq { "" }; result1 ] |> Seq.concat

        Analyser.create createEmpty collect postprocess print

    /// Combine preferences
    let combinePrefs prefs1 prefs2 = (prefs2, prefs1)

    let analyseWith prefs setData analyser data =
        (analyser.emptyData (), data)
        ||> Seq.fold (analyser.collect prefs)
        |> analyser.postprocess setData
        |> analyser.print prefs

    let analyse setData config data =
        let basicPrefs: BasicAnalyser.Preferences = { numBase = config.numBase }

        let setPrefs: SetAnalyser.Preferences =
            {
                missingPercent = config.missingPercent
                numBase = config.numBase
                withFoils = config.setWithFoils
            }

        let langPrefs: LanguageAnalyser.Preferences = { numBase = config.numBase }

        // Combine all analysers
        let analyser =
            BasicAnalyser.get
            |> combine SetAnalyser.get
            |> combine LanguageAnalyser.get

        // Combine preferences
        let prefs =
            basicPrefs
            |> combinePrefs setPrefs
            |> combinePrefs langPrefs

        analyseWith prefs setData analyser data
