namespace MagicCollectionHelper.AvaloniaApp.Components.Inventory

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Media

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.ViewComponents
open MagicCollectionHelper.AvaloniaApp.ViewComponent
open MagicCollectionHelper.AvaloniaApp.ViewHelper

module View =
    let actionBar (infoMap: CardInfoMap) (entries: 'a list) (state: State) (dispatch: Dispatch) =
        ActionButtonBar.create [
            ActionButton.create {
                text = "Take inventory"
                isEnabled = (not (infoMap.IsEmpty || entries.IsEmpty || state.viewMode = Loading))
                action = (fun _ -> TakeInventory |> dispatch)
                subPatch = Never
            }
        ]

    type LocCards = {
        location: InventoryLocation
        amount: uint
        cards: string seq
    }

    let cardItem prefs (state: State) (entry: AgedEntryWithInfo) =
        let amount = entry ^. AgedEntryWithInfoOptic.amount
        let amountOld = entry ^. AgedEntryWithInfoOptic.amountOld
        let foil = entry ^. AgedEntryWithInfoOptic.foil
        let name = entry ^. AgedEntryWithInfoOptic.name

        let langValue = entry ^. AgedEntryWithInfoOptic.language |> Language.unwrap

        let numberValue = entry ^. AgedEntryWithInfoOptic.number |> CollectorNumber.unwrap

        let setValue = entry ^. AgedEntryWithInfoOptic.set |> MagicSet.unwrap

        let star = if foil then "★" else " "
        let old = amount = amountOld

        let brush =
            let searched = Search.fits state.search entry

            match searched, old with
            | true, true -> Brushes.White
            | false, true -> Brushes.DarkGray
            | true, false -> Brushes.LimeGreen
            | false, false -> Brushes.DarkGreen

        let inline pN p = Numbers.print prefs.numBase p

        let added =
            if not old then
                $"(+%2s{pN 0 (amount - amountOld)})"
            else
                "     "

        CheckBox.create [
            CheckBox.fontFamily Config.monospaceFont
            CheckBox.foreground brush
            CheckBox.content (
                $"{star}[%5s{setValue}-%s{numberValue.PadLeft(3, '0')}]-{langValue}"
                + $" {added}%2s{pN 0 amount} {name}"
            )
        ]

    let wrapLayer withBorder children =
        let stack =
            StackPanel.create [ StackPanel.spacing 4.; StackPanel.children children ] :> IView

        if withBorder then
            Border.create [ Border.padding (8., 0.); Border.child stack ] :> IView
        else
            stack

    let renderEntryList prefs state entries =
        wrapLayer true [
            for entry in entries do
                cardItem prefs state entry
        ]

    let rec renderEntryTree prefs state dispatch first tree =
        let renderEntryTree = renderEntryTree prefs state dispatch false

        let inline pN p = Numbers.print prefs.numBase p

        match tree with
        // If we have only one node, we skip it
        | Nodes [ (_, child) ] -> renderEntryTree child
        | Nodes nodes ->
            let listWithPreAndSuccessor =
                List.windowed 3 [ None; yield! List.map Some nodes; None ]
                |> List.map (function
                    | [ predecessor; Some item; successor ] -> predecessor, item, successor
                    | _ -> failwith "This should never happen")

            let mutable firstNotToRender = true

            wrapLayer (not first) [
                for (predecessor, (name, child), successor) in listWithPreAndSuccessor do
                    // We want to add the amount
                    let amount = ExpanderTree.sumUpCards state.search child
                    let name = $"%s{name} ({pN 0 amount})"

                    // We don't render it, if itself, the predecessor and the successor are empty
                    let render =
                        amount > 0u
                        || match predecessor with
                           | Some predecessor -> ExpanderTree.sumUpCards state.search (snd predecessor) > 0u
                           | None -> false
                        || match successor with
                           | Some successor -> ExpanderTree.sumUpCards state.search (snd successor) > 0u
                           | None -> false

                    if render then
                        firstNotToRender <- true

                        Expander.create [
                            Expander.header name
                            Expander.isExpanded (amount > 0u)
                            Expander.content (renderEntryTree child)
                        ]
                    else if firstNotToRender then
                        firstNotToRender <- false

                        Expander.create [
                            Expander.header "... (0)"
                            Expander.isExpanded false
                            Expander.content
                                "Not shown because there are no results for the current search in this cluster"
                        ]
            ]
        | Leaf entries -> renderEntryList prefs state entries

    let renderEntryTreeForLocation prefs state dispatch (location: InventoryLocation) trees =
        ScrollViewer.create [
            ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
            ScrollViewer.content (
                Border.create [
                    Border.padding (10., 10.)
                    Border.child (renderEntryTree prefs state dispatch true trees)
                ]
            )
        ]

    let locationItem prefs state dispatch (location: InventoryLocation) tree =
        Border.create [
            Border.borderThickness (1., 0., 0., 0.)
            Border.borderBrush Config.lineColor
            Border.child (
                DockPanel.create [
                    DockPanel.children [
                        SearchBar.render state.search dispatch
                        renderEntryTreeForLocation prefs state dispatch location tree
                    ]
                ]
            )
        ]
        :> IView

    let content prefs (state: State) (dispatch: Dispatch) : IView =
        match state.viewMode with
        | Empty ->
            Border.create [
                Border.padding 10.
                Border.child (TextBlock.create [ TextBlock.text "Press 'Take inventory' to start processing." ])
            ]
            :> IView
        | Edit -> LocationEdit.render state dispatch
        | Loading ->
            Border.create [
                Border.padding 10.
                Border.child (TextBlock.create [ TextBlock.text "Loading..." ])
            ]
            :> IView
        | Location location ->
            let locations = state.filteredInventory
            let locationMap = locations |> Map.ofList

            let inline pN p = Numbers.print prefs.numBase p

            let nameFromLocation map (location: InventoryLocation) =
                let amount = Map.find location map |> ExpanderTree.sumUpCards state.search |> pN 0

                match location with
                | Custom location -> $"{location.name} ({amount})"
                | Fallback -> $"Leftover ({amount})"

            let current = location |> Option.defaultValue (locations |> List.head |> fst)

            TabView.renderFromList
                (nameFromLocation locationMap)
                (locationItem prefs state dispatch)
                (ChangeLocation >> dispatch)
                TabView.Left
                locations
                current

    let render prefs (infoMap: CardInfoMap) entries (state: State) (dispatch: Dispatch) : IView =
        DockPanel.create [
            DockPanel.children [ actionBar infoMap entries state dispatch; content prefs state dispatch ]
        ]
        :> IView

    open Avalonia.FuncUI
    open Avalonia.FuncUI.Elmish.ElmishHook

    type Input = {
        cardInfo: IReadable<CardInfoMap>
        entries: IReadable<AgedEntry list>
        prefs: IReadable<Prefs>
        setData: IReadable<SetDataMap>
    }

    let view input =
        Component.create (
            "inventory-tab",
            fun ctx ->
                let prefs = ctx.usePassedRead input.prefs
                let infoMap = ctx.usePassedRead input.cardInfo
                let entries = ctx.usePassedRead input.entries
                let setData = ctx.usePassedRead input.setData

                let state, dispatch =
                    ctx.useElmish (Model.init, Update.perform prefs setData infoMap entries)

                render prefs.Current infoMap.Current entries.Current state dispatch
        )
