namespace MagicCollectionHelper.Core

open SimpleOptics

[<AutoOpen>]
module CardTypesModules =
    [<RequireQualifiedAccess>]
    module Card =
        let create foil language number set = {
            foil = foil
            language = language
            number = number
            set = set
        }

        let isExactSame (card1: Card) (card2: Card) =
            card1 ^. CardOptic.set = card2 ^. CardOptic.set
            && card1 ^. CardOptic.number = card2 ^. CardOptic.number

        let isToken (card: Card) =
            let setValue = card ^. CardOptic.set |> MagicSet.unwrap

            [ String.length >> (=) 4; String.startsWith "T" ]
            |> List.forall (fun fnc -> fnc setValue)

    [<RequireQualifiedAccess>]
    module Entry =
        let create amount card : Entry = { amount = amount; card = card }

        /// Takes a list of cards and creates entry out of equal cards
        let collapseCardList cardList =
            cardList
            |> List.fold
                (fun cardAmountMap card ->
                    Map.change card (fun mapEntry -> mapEntry |> Option.defaultValue 0u |> (+) 1u |> Some) cardAmountMap)
                Map.empty
            |> Map.toList
            |> List.map (fun (card, amount) -> { card = card; amount = amount })

    [<RequireQualifiedAccess>]
    module Oldable =
        let create old data = { old = old; data = data }

        let map mapper wrapper =
            create wrapper.old (mapper wrapper.data)

    [<RequireQualifiedAccess>]
    module OldAmountable =
        let create amountOld data : OldAmountable<'a> = { amountOld = amountOld; data = data }

        let inline map mapper (wrapper: OldAmountable<'a>) : OldAmountable<'b> =
            create wrapper.amountOld (mapper wrapper.data)

    [<RequireQualifiedAccess>]
    module WithInfo =
        let create info data = { data = data; info = info }

        let addInfo infoMap getSet getNumber data =
            Map.tryFind (getSet data, getNumber data) infoMap
            |> Option.map (fun info -> create info data)

        let inline map mapper (wrapper: WithInfo<'a>) : WithInfo<'b> =
            create wrapper.info (mapper wrapper.data)

        /// Checks if those two cards are rulewise the same. They do not have to be from the same set
        let isSame withInfo1 withInfo2 =
            withInfo1.info.oracleId = withInfo2.info.oracleId

    [<RequireQualifiedAccess>]
    module CardWithInfo =
        let create card info : CardWithInfo = { data = card; info = info }

        let fromCard infoMap =
            WithInfo.addInfo infoMap (Optic.get CardOptic.set) (Optic.get CardOptic.number)

        /// Checks if those two cards are rulewise the same. They do not have to be from the same set
        let isSame (card1: CardWithInfo) (card2: CardWithInfo) = WithInfo.isSame card1 card2

        let isExactSame (card1: CardWithInfo) (card2: CardWithInfo) = Card.isExactSame card1.data card2.data

    [<RequireQualifiedAccess>]
    module EntryWithInfo =
        let create entry info : EntryWithInfo = { data = entry; info = info }

        let fromEntry infoMap =
            WithInfo.addInfo infoMap (Optic.get EntryOptic.set) (Optic.get EntryOptic.number)

    [<RequireQualifiedAccess>]
    module AgedCard =
        let create old card : AgedCard = Oldable.create old card

    [<RequireQualifiedAccess>]
    module AgedEntry =
        let create amountOld entry : AgedEntry = OldAmountable.create amountOld entry

        let determineCardAge oldList newList =
            // We first create a map
            let oldMap =
                oldList |> List.map (fun (entry: Entry) -> entry.card, entry) |> Map.ofList

            newList
            |> List.map (fun (entry: Entry) ->
                let amountOld =
                    match Map.tryFind entry.card oldMap with
                    | Some entry -> entry.amount
                    | None -> 0u

                OldAmountable.create amountOld entry)

    [<RequireQualifiedAccess>]
    module AgedCardWithInfo =
        let create info card : AgedCardWithInfo = WithInfo.create info card

        let fromCard infoMap =
            WithInfo.addInfo infoMap (Optic.get AgedCardOptic.set) (Optic.get AgedCardOptic.number)

        let removeAge = WithInfo.map (Optic.get AgedCardOptic.card)

    [<RequireQualifiedAccess>]
    module AgedEntryWithInfo =
        let create info entry : AgedEntryWithInfo = WithInfo.create info entry

        let fromEntry infoMap =
            WithInfo.addInfo infoMap (Optic.get AgedEntryOptic.set) (Optic.get AgedEntryOptic.number)

        let removeAge = WithInfo.map (Optic.get AgedEntryOptic.entry)

        /// Takes a list of cards and creates entry out of equal cards
        let fromCardList (cardList: AgedCardWithInfo list) =
            cardList
            |> List.fold
                (fun cardAmountMap card ->
                    Map.change
                        (AgedCardWithInfo.removeAge card)
                        (fun mapEntry ->
                            mapEntry
                            |> Option.defaultValue (0u, 0u)
                            |> (fun (a, b) -> a + 1u, b + if card.data.old then 1u else 0u)
                            |> Some)
                        cardAmountMap)
                Map.empty
            |> Map.toList
            |> List.map (fun (cardWithInfo: CardWithInfo, (amount, amountOld)) ->
                cardWithInfo.data
                |> Entry.create amount
                |> AgedEntry.create amountOld
                |> create cardWithInfo.info)
