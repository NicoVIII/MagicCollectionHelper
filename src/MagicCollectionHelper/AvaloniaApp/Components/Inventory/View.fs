module MagicCollectionHelper.AvaloniaApp.Components.Inventory.View

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Media

open MagicCollectionHelper.Core

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.ViewComponents
open MagicCollectionHelper.AvaloniaApp.Elements
open MagicCollectionHelper.AvaloniaApp.ViewHelper

let actionBar (infoMap: CardInfoMap) (entries: 'a list) (state: State) (dispatch: Dispatch) =
    ActionButtonBar.create [
        ActionButton.create
            { text = "Take inventory"
              isEnabled =
                  (not (
                      infoMap.IsEmpty
                      || entries.IsEmpty
                      || state.viewMode = Loading
                  ))
              action = (fun _ -> TakeInventory |> dispatch)
              subPatch = Never }
    ]

type LocCards =
    { location: InventoryLocation
      amount: uint
      cards: string seq }

let cardItem (state: State) (entry: AgedEntryWithInfo) =
    let amount = entry ^. AgedEntryWithInfoLenses.amount
    let amountOld = entry ^. AgedEntryWithInfoLenses.amountOld
    let foil = entry ^. AgedEntryWithInfoLenses.foil
    let name = entry ^. AgedEntryWithInfoLenses.name

    let langValue =
        entry ^. AgedEntryWithInfoLenses.language
        |> Language.unwrap

    let numberValue =
        entry ^. AgedEntryWithInfoLenses.number
        |> CollectorNumber.unwrap

    let setValue =
        entry ^. AgedEntryWithInfoLenses.set
        |> MagicSet.unwrap

    let star = if foil then "★" else " "
    let old = amount = amountOld

    let brush =
        let searched = Search.fits state.search entry

        match searched, old with
        | true, true -> Brushes.White
        | false, true -> Brushes.DarkGray
        | true, false -> Brushes.LimeGreen
        | false, false -> Brushes.DarkGreen

    let added = if not old then $"(+%2i{amount - amountOld})" else "     "

    CheckBox.create [
        CheckBox.fontFamily Config.monospaceFont
        CheckBox.foreground brush
        CheckBox.content (
            $"{star}[%5s{setValue}-%s{numberValue.PadLeft(3, '0')}]-{langValue}"
            + $" {added}%2i{amount} {name}"
        )
    ]

let wrapLayer withBorder children =
    let stack =
        StackPanel.create [
            StackPanel.spacing 4.
            StackPanel.children children
        ]
        :> IView

    if withBorder then
        Border.create [
            Border.padding (8., 0.)
            Border.child stack
        ]
        :> IView
    else
        stack

let renderEntryList state entries =
    wrapLayer
        true
        [ for entry in entries do
              cardItem state entry ]

let rec renderEntryTree state dispatch first tree =
    let renderEntryTree = renderEntryTree state dispatch false

    match tree with
    // If we have only one node, we skip it
    | Nodes [ (_, child) ] -> renderEntryTree child
    | Nodes nodes ->
        wrapLayer
            (not first)
            [ for (name: string, child) in nodes do
                  // We want to add the amount
                  let amount = ExpanderTree.sumUpCards state.search child
                  let name = $"{name} ({amount})"

                  Expander.create [
                      Expander.header name
                      Expander.isExpanded (amount > 0u)
                      Expander.content (renderEntryTree child)
                  ] ]
    | Leaf entries -> renderEntryList state entries

let renderEntryTreeForLocation state dispatch (location: InventoryLocation) trees =
    ScrollViewer.create [
        ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
        ScrollViewer.content (
            Border.create [
                Border.padding (10., 10.)
                Border.child (renderEntryTree state dispatch true trees)
            ]
        )
    ]

let locationItem state dispatch (location: InventoryLocation) tree =
    Border.create [
        Border.borderThickness (1., 0., 0., 0.)
        Border.borderBrush Config.lineColor
        Border.child (
            DockPanel.create [
                DockPanel.children [
                    SearchBar.render state.search dispatch
                    renderEntryTreeForLocation state dispatch location tree
                ]
            ]
        )
    ]
    :> IView

let content (state: State) (dispatch: Dispatch) : IView =
    match state.viewMode with
    | Empty ->
        Border.create [
            Border.padding 10.
            Border.child (
                TextBlock.create [
                    TextBlock.text "Press 'Take inventory' to start processing."
                ]
            )
        ]
        :> IView
    | Edit -> LocationEdit.render state dispatch
    | Loading ->
        Border.create [
            Border.padding 10.
            Border.child (
                TextBlock.create [
                    TextBlock.text "Loading..."
                ]
            )
        ]
        :> IView
    | Location location ->
        let locations = state.filteredInventory
        let locationMap = locations |> Map.ofList

        let nameFromLocation map (location: InventoryLocation) =
            let amount =
                Map.find location map
                |> ExpanderTree.sumUpCards state.search

            match location with
            | Custom location -> $"{location.name} ({amount})"
            | Fallback -> $"Leftover ({amount})"

        let current =
            location
            |> Option.defaultValue (locations |> List.head |> fst)

        TabView.renderFromList
            (nameFromLocation locationMap)
            (locationItem state dispatch)
            (ChangeLocation >> dispatch)
            TabView.Left
            locations
            current

let render (infoMap: CardInfoMap) setData entries (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            actionBar infoMap entries state dispatch
            content state dispatch
        ]
    ]
    :> IView
