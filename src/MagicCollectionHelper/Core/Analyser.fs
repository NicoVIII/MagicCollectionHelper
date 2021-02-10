namespace MagicCollectionHelper.Core

open MagicCollectionHelper.Core.Types

module Analyser =
    /// Combine analysers to use same loop for collection
    let combine analyser1 analyser2 =
        let createEmpty () =
            (analyser2.emptyData (), analyser1.emptyData ())

        let collect (settings2, settings1) (data2, data1) entry =
            let result1 = analyser1.collect settings1 data1 entry
            let result2 = analyser2.collect settings2 data2 entry
            (result2, result1)

        let postprocess setData (data2, data1) =
            let result1 = analyser1.postprocess setData data1
            let result2 = analyser2.postprocess setData data2
            (result2, result1)

        let print (settings2, settings1) (result2, result1) =
            let result1 = analyser1.print settings1 result1
            let result2 = analyser2.print settings2 result2
            [ result2; seq { "" }; result1 ] |> Seq.concat

        Analyser.create createEmpty collect postprocess print

    /// Combine settings
    let combineSettings settings1 settings2 = (settings2, settings1)

    let analyseWith settings setData analyser data =
        (analyser.emptyData (), data)
        ||> Seq.fold (analyser.collect settings)
        |> analyser.postprocess setData
        |> analyser.print settings

    let analyse setData config data =
        let basicSettings: BasicAnalyser.Settings = { dozenalize = config.dozenalize }

        let setSettings: SetAnalyser.Settings =
            { missingPercent = config.missingPercent
              dozenalize = config.dozenalize
              withFoils = config.setWithFoils }

        let langSettings: LanguageAnalyser.Settings = { dozenalize = config.dozenalize }

        // Combine all analysers
        let analyser =
            BasicAnalyser.get
            |> combine SetAnalyser.get
            |> combine LanguageAnalyser.get

        // Combine settings
        let settings =
            basicSettings
            |> combineSettings setSettings
            |> combineSettings langSettings

        analyseWith settings setData analyser data
