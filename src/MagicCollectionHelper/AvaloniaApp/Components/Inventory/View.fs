module MagicCollectionHelper.AvaloniaApp.Components.Inventory.View

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Media
open System

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.ViewComponents
open MagicCollectionHelper.AvaloniaApp.Elements
open MagicCollectionHelper.AvaloniaApp.ViewHelper

let actionBar (infoMap: CardInfoMap) (entries: CardEntry list) (state: State) (dispatch: Dispatch) : IView =
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

let cardItem (state: State) (entryWithInfo: CardEntryWithInfo) =
    let entry = entryWithInfo.entry
    let info = entryWithInfo.info
    let set = entry.card.set
    let number = entry.card.number
    let name = info.name

    let star = if entry.card.foil then "★" else " "

    let brush =
        if String.iContains name state.search then
            Brushes.White
        else
            Brushes.DarkGray

    CheckBox.create [
        CheckBox.fontFamily Config.monospaceFont
        CheckBox.foreground brush
        CheckBox.content (
            $"{star}[%5s{set.Value}-%s{number.Value.PadLeft(3, '0')}]-{entry.card.language.Value}"
            + $" %2i{entry.amount} {name}"
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
                  Expander.create [
                      Expander.header name
                      Expander.isExpanded true
                      Expander.content (renderEntryTree child)
                  ] ]
    | Leaf entries -> renderEntryList state entries

let renderEntryTreeForLocation state dispatch (location: InventoryLocation) trees =
    let sortBy =
        match location with
        | Fallback -> []
        | Custom location -> location.sortBy

    ScrollViewer.create [
        ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
        ScrollViewer.content (
            Border.create [
                Border.padding (10., 10.)
                Border.child (renderEntryTree state dispatch true trees)
            ]
        )
    ]

let searchBar state dispatch =
    TextBox.create [
        TextBox.dock Dock.Top
        TextBox.text state.search
        TextBox.onTextChanged (
            (fun text ->
                if state.search <> text then
                    ChangeSearchString text |> dispatch),
            OnChangeOf state.search
        )
    ]

let locationItem state dispatch (location: InventoryLocation) tree =
    Border.create [
        Border.borderThickness (1., 0., 0., 0.)
        Border.borderBrush Config.lineColor
        Border.child (
            DockPanel.create [
                DockPanel.children [
                    searchBar state dispatch
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
            let rec sumUpTree tree =
                match tree with
                | Nodes nodes -> List.sumBy (snd >> sumUpTree) nodes
                | Leaf entries -> List.length entries

            let amount = Map.find location map |> sumUpTree

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

let render (infoMap: CardInfoMap) setData (entries: CardEntry list) (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            actionBar infoMap entries state dispatch
            content state dispatch
        ]
    ]
    :> IView
