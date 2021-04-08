module MagicCollectionHelper.AvaloniaApp.Components.Inventory.View

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.Components
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open System

open MagicCollectionHelper.Core.Types

open MagicCollectionHelper.AvaloniaApp
open MagicCollectionHelper.AvaloniaApp.Components.Inventory
open MagicCollectionHelper.AvaloniaApp.Components.Inventory.ViewComponents
open MagicCollectionHelper.AvaloniaApp.Elements

let topBar (infoMap: CardInfoMap) (entries: CardEntry list) (state: State) (dispatch: Dispatch) : IView =
    ActionButtonBar.create [
        ActionButton.create
            { text = "Take inventory"
              isEnabled =
                  (not (
                      infoMap.IsEmpty
                      || entries.IsEmpty
                      || state.loadInProgress
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

let locationItem setData (location: InventoryLocation) entries =
    let amount =
        List.sumBy (fun (e: CardEntryWithInfo) -> e.entry.amount) entries

    let entries = sortEntries setData location entries

    Expander.create [
        Expander.header (
            match location with
            | Custom location -> $"{location.name} ({amount})"
            | Fallback -> $"Leftover ({amount})"
        )
        Expander.content (
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

let content (infoMap: CardInfoMap) setData (state: State) (dispatch: Dispatch) : IView =
    match state.editLocations, state.loadInProgress with
    | true, _ -> LocationEdit.render state dispatch
    | false, true ->
        Border.create [
            Border.padding 10.
            Border.child (
                TextBlock.create [
                    TextBlock.text "Loading..."
                ]
            )
        ]
        :> IView
    | false, false ->
        let locations =
            state.inventory
            |> Map.toList
            |> List.sortBy
                (fun (location, _) ->
                    // We sort like our locations are sorted
                    Map.tryPick
                        (fun _ l ->
                            if Custom l = location then
                                Some l.position
                            else
                                None)
                        state.locations
                    |> Option.defaultValue 999u)
            |> List.map
                (fun (location, entries) ->
                    let cards =
                        entries
                        |> List.choose
                            (fun entry ->
                                Map.tryFind (entry.card.set, entry.card.number) infoMap
                                |> Option.map (CardEntryWithInfo.create entry))

                    (location, cards))

        StackPanel.create [
            StackPanel.children [
                for (location, cards) in locations do
                    locationItem setData location cards
            ]
        ]
        :> IView

let render (infoMap: CardInfoMap) setData (entries: CardEntry list) (state: State) (dispatch: Dispatch) : IView =
    DockPanel.create [
        DockPanel.children [
            topBar infoMap entries state dispatch
            Button.create [
                Button.dock Dock.Top
                Button.content (
                    if state.editLocations then
                        "Close"
                    else
                        "Edit"
                )
                Button.onClick (
                    (fun _ ->
                        (if state.editLocations then
                             CloseLocationEdit
                         else
                             OpenLocationEdit)
                        |> dispatch),
                    Always
                )
            ]
            ScrollViewer.create [
                ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                ScrollViewer.content (content infoMap setData state dispatch)
            ]
        ]
    ]
    :> IView
