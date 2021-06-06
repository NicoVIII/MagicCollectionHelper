module MagicCollectionHelper.AvaloniaApp.Components.Inventory.View

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Input
open System

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.ViewComponents
open MagicCollectionHelper.AvaloniaApp.Elements
open MagicCollectionHelper.Core.Types.Generated

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
              action = (fun _ -> TakeInventory |> dispatch) }
    ]

type LocCards =
    { location: InventoryLocation
      amount: uint
      cards: string seq }

let cardItem (entryWithInfo: CardEntryWithInfo) =
    let entry = entryWithInfo.entry
    let info = entryWithInfo.info
    let set = entry.card.set
    let number = entry.card.number
    let name = info.name

    let star = if entry.card.foil then "★" else " "

    CheckBox.create [
        CheckBox.fontFamily Config.monospaceFont
        CheckBox.content (
            $"{star}[%5s{set.Value}-%s{number.Value.PadLeft(3, '0')}]-{entry.card.language.Value}"
            + $" %2i{entry.amount} {name}"
        )
    ]

let getSortByValue setData (entryWithInfo: CardEntryWithInfo) sortBy =
    let entry = entryWithInfo.entry
    let info = entryWithInfo.info

    match sortBy with
    | ByColorIdentity ->
        let pos =
            ColorIdentity.getPosition info.colorIdentity

        sprintf "%02i" pos
    | ByName -> info.name
    | BySet ->
        let date =
            Map.tryFind entry.card.set setData
            |> function
            | Some setData -> setData.date
            | None -> "0000-00-00"

        let extension =
            match entry.card.set.Value with
            | set when set.StartsWith "T" -> set.Substring 1 + "Z"
            | set -> set + "A"

        $"{date}{extension}"
    | ByCollectorNumber -> sprintf "%s" (entry.card.number.Value.PadLeft(3, '0'))
    | ByCmc -> sprintf "%02i" info.cmc
    | ByTypeContains typeContains ->
        typeContains
        |> List.fold
            (fun (found, strng) typeContains ->
                if found then
                    (true, strng + "9")
                else if info.typeLine.Contains typeContains then
                    (true, strng + "1")
                else
                    (false, strng + "9"))
            (false, "")
        |> snd
    | ByRarity rarities ->
        rarities
        |> List.indexed
        |> List.tryPick
            (fun (index, raritySet) ->
                if Set.contains info.rarity raritySet then
                    Some index
                else
                    None)
        |> Option.defaultValue (List.length rarities)
        |> string
    | ByLanguage language ->
        language
        |> List.indexed
        |> List.tryPick
            (fun (index, language) ->
                if language = entry.card.language then
                    Some index
                else
                    None)
        |> Option.defaultValue (List.length language)
        |> string

let sortEntries setData location (entries: CardEntryWithInfo list) =
    let random = Random()

    let sortRules =
        match location with
        | Custom location -> location.sortBy
        | Fallback -> [ ByName ]

    let sortBy (e: CardEntryWithInfo) =
        sortRules
        |> List.map (getSortByValue setData e)
        // We add a random factor at the end
        |> (fun lst -> List.append lst [ random.Next(0, 10000) |> sprintf "%04i" ])

    List.sortBy sortBy entries

let renderEntryList entries =
    ScrollViewer.create [
        ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
        ScrollViewer.content (
            Border.create [
                Border.padding (20., 10.)
                Border.child (
                    StackPanel.create [
                        StackPanel.spacing 4.
                        StackPanel.children [
                            for entry in entries do
                                cardItem entry
                        ]
                    ]
                )
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
        TextBox.onKeyUp
            (fun args ->
                if args.Key = Key.Enter then
                    Search |> dispatch)
    ]

let locationItem setData state dispatch (location: InventoryLocation) entries =
    let entries = sortEntries setData location entries

    Border.create [
        Border.borderThickness (1., 0., 0., 0.)
        Border.borderBrush Config.lineColor
        Border.child (
            DockPanel.create [
                DockPanel.children [
                    searchBar state dispatch
                    renderEntryList entries
                ]
            ]
        )
    ]
    :> IView

let content setData (state: State) (dispatch: Dispatch) : IView =
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
                |> List.sumBy (fun (e: CardEntryWithInfo) -> e.entry.amount)

            match location with
            | Custom location -> $"{location.name} ({amount})"
            | Fallback -> $"Leftover ({amount})"

        let current =
            location
            |> Option.defaultValue (locations |> List.head |> fst)

        TabView.renderFromMap
            (nameFromLocation locationMap)
            (locationItem setData state dispatch)
            (ChangeLocation >> dispatch)
            TabView.Left
            locations
            current

let render (infoMap: CardInfoMap) setData (entries: CardEntry list) (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            actionBar infoMap entries state dispatch
            content setData state dispatch
        ]
    ]
    :> IView
