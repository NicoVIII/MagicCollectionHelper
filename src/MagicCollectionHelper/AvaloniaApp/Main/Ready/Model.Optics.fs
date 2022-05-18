namespace MagicCollectionHelper.AvaloniaApp.Main.Ready

open SimpleOptics

[<RequireQualifiedAccess>]
module CommonStateOptic =
    let analyseText =
        Lens((fun (state: CommonState) -> state.analyseText), (fun state value -> { state with analyseText = value }))

    let cardInfo =
        Lens((fun (state: CommonState) -> state.cardInfo), (fun state value -> { state with cardInfo = value }))

    let dsEntries =
        Lens((fun (state: CommonState) -> state.dsEntries), (fun state value -> { state with dsEntries = value }))

    let entries =
        Lens((fun (state: CommonState) -> state.entries), (fun state value -> { state with entries = value }))

    let prefs =
        Lens((fun (state: CommonState) -> state.prefs), (fun state value -> { state with prefs = value }))

    let setData =
        Lens((fun (state: CommonState) -> state.setData), (fun state value -> { state with setData = value }))

    let viewMode =
        Lens((fun (state: CommonState) -> state.viewMode), (fun state value -> { state with viewMode = value }))

[<RequireQualifiedAccess>]
module StateOptic =
    let common =
        Lens((fun (state: State) -> state.common), (fun (state: State) value -> { state with common = value }))

    let collection =
        Lens((fun (state: State) -> state.collection), (fun (state: State) value -> { state with collection = value }))

    let inventory =
        Lens((fun (state: State) -> state.inventory), (fun (state: State) value -> { state with inventory = value }))

    let analyseText = Optic.compose common CommonStateOptic.analyseText

    let cardInfo = Optic.compose common CommonStateOptic.cardInfo

    let dsEntries = Optic.compose common CommonStateOptic.dsEntries

    let entries = Optic.compose common CommonStateOptic.entries

    let prefs = Optic.compose common CommonStateOptic.prefs

    let setData = Optic.compose common CommonStateOptic.setData

    let viewMode = Optic.compose common CommonStateOptic.viewMode
