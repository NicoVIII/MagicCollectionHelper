module MagicCollectionHelper.AvaloniaApp.Components.Inventory.View

open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.FuncUI.Components
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types

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

let cardItem (infoMap: CardInfoMap) (entry: CardEntry) =
    let set = entry.card.set
    let number = entry.card.number
    let info = infoMap.TryFind(set, number)

    let name =
        match info with
        | Some info -> info.name
        | None -> "???"

    CheckBox.create [
        CheckBox.fontFamily Config.monospaceFont
        CheckBox.content $"[%5s{set.Value}-%03i{number.Value}] %2i{entry.amount} {name}"
    ]

let getSortByValue (e: CardEntry, i: CardInfo option) sortBy =
    match sortBy with
    | ByName ->
        match i with
        | Some info -> info.name
        | None -> "???"
    | BySet ->
        match e.card.set.Value with
        | set when set.StartsWith "T" -> set.Substring 1 + "Z"
        | set -> set + "A"
    | ByCollectorNumber -> $"%03i{e.card.number.Value}"

let sortEntries (infoMap: CardInfoMap) location (entries: CardEntry list) =
    let sortRules =
        match location with
        | Custom location -> location.sortBy
        | Fallback -> [ ByName ]

    let sortBy (e: CardEntry) =
        let info =
            infoMap.TryFind(e.card.set, e.card.number)

        sortRules |> List.map (getSortByValue (e, info))

    List.sortBy sortBy entries

let locationItem (infoMap: CardInfoMap) (location: InventoryLocation, entries) =
    let amount =
        List.sumBy (fun (e: CardEntry) -> e.amount) entries

    let entries = sortEntries infoMap location entries

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
                                cardItem infoMap entry
                        ]
                    ]
                )
            ]
        )
    ]

let content (infoMap: CardInfoMap) (state: State) (dispatch: Dispatch) : IView =
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

        StackPanel.create [
            StackPanel.children [
                for location in locations do
                    locationItem infoMap location
            ]
        ]
        :> IView

let render (infoMap: CardInfoMap) (entries: CardEntry list) (state: State) (dispatch: Dispatch) : IView =
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
                    SubPatchOptions.Always
                )
            ]
            ScrollViewer.create [
                ScrollViewer.horizontalScrollBarVisibility ScrollBarVisibility.Disabled
                ScrollViewer.content (content infoMap state dispatch)
            ]
        ]
    ]
    :> IView
