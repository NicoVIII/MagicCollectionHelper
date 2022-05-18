namespace MagicCollectionHelper.Core

open SimpleOptics

[<AutoOpen>]
module CardTypesOptics =
    [<RequireQualifiedAccess>]
    module CardOptic =
        let foil =
            Lens((fun (card: Card) -> card.foil), (fun card value -> { card with foil = value }))

        let language =
            Lens((fun (card: Card) -> card.language), (fun card value -> { card with language = value }))

        let number =
            Lens((fun (card: Card) -> card.number), (fun card value -> { card with number = value }))

        let set =
            Lens((fun (card: Card) -> card.set), (fun card value -> { card with set = value }))

    [<RequireQualifiedAccess>]
    module CardInfoOptic =
        let cmc =
            Lens((fun (cardInfo: CardInfo) -> cardInfo.cmc), (fun cardInfo value -> { cardInfo with cmc = value }))

        let colorIdentity =
            Lens(
                (fun (cardInfo: CardInfo) -> cardInfo.colorIdentity),
                (fun cardInfo value -> { cardInfo with colorIdentity = value })
            )

        let name =
            Lens((fun (cardInfo: CardInfo) -> cardInfo.name), (fun cardInfo value -> { cardInfo with name = value }))

        let rarity =
            Lens(
                (fun (cardInfo: CardInfo) -> cardInfo.rarity),
                (fun cardInfo value -> { cardInfo with rarity = value })
            )

        let typeLine =
            Lens(
                (fun (cardInfo: CardInfo) -> cardInfo.typeLine),
                (fun cardInfo value -> { cardInfo with typeLine = value })
            )

    [<RequireQualifiedAccess>]
    module EntryOptic =
        let amount =
            Lens((fun (entry: Entry) -> entry.amount), (fun entry value -> { entry with amount = value }))

        let card =
            Lens((fun entry -> entry.card), (fun entry value -> { entry with card = value }))

        let foil = Optic.compose card CardOptic.foil
        let language = Optic.compose card CardOptic.language
        let number = Optic.compose card CardOptic.number
        let set = Optic.compose card CardOptic.set

    [<RequireQualifiedAccess>]
    module OldableOptic =
        let old =
            Lens((fun oldable -> oldable.old), (fun oldable value -> { oldable with old = value }))

        let data =
            Lens((fun (oldable: Oldable<'a>) -> oldable.data), (fun oldable value -> { oldable with data = value }))

    [<RequireQualifiedAccess>]
    module OldAmountableOptic =
        let amountOld =
            Lens(
                (fun oldAmountable -> oldAmountable.amountOld),
                (fun oldAmountable value -> { oldAmountable with amountOld = value })
            )

        let data =
            Lens(
                (fun (oldAmountable: OldAmountable<'a>) -> oldAmountable.data),
                (fun oldAmountable value -> { oldAmountable with data = value })
            )

    [<RequireQualifiedAccess>]
    module WithInfo =
        let data =
            Lens((fun withInfo -> withInfo.data), (fun withInfo value -> { withInfo with data = value }))

        let info =
            Lens((fun withInfo -> withInfo.info), (fun withInfo value -> { withInfo with info = value }))

    [<RequireQualifiedAccess>]
    module AgedCardOptic =
        // Base Optic
        let card: Lens<AgedCard, Card> = OldableOptic.data
        let old: Lens<AgedCard, bool> = OldableOptic.old

        // Card Optic
        let foil = Optic.compose card CardOptic.foil
        let language = Optic.compose card CardOptic.language
        let number = Optic.compose card CardOptic.number
        let set = Optic.compose card CardOptic.set

    [<RequireQualifiedAccess>]
    module AgedEntryOptic =
        // Base Optic
        let entry: Lens<AgedEntry, Entry> = OldAmountableOptic.data
        let amountOld: Lens<AgedEntry, uint> = OldAmountableOptic.amountOld

        // CardEntry Optic
        let amount = Optic.compose entry EntryOptic.amount
        let card = Optic.compose entry EntryOptic.card
        let foil = Optic.compose entry EntryOptic.foil
        let language = Optic.compose entry EntryOptic.language
        let number = Optic.compose entry EntryOptic.number
        let set = Optic.compose entry EntryOptic.set

    [<RequireQualifiedAccess>]
    module CardWithInfoOptic =
        // Base Optic
        let card: Lens<CardWithInfo, Card> = WithInfo.data
        let info: Lens<CardWithInfo, CardInfo> = WithInfo.info

        // Card Optic
        let foil = Optic.compose card CardOptic.foil
        let language = Optic.compose card CardOptic.language
        let number = Optic.compose card CardOptic.number
        let set = Optic.compose card CardOptic.set

        // Info Optic
        let cmc = Optic.compose info CardInfoOptic.cmc
        let colorIdentity = Optic.compose info CardInfoOptic.colorIdentity
        let name = Optic.compose info CardInfoOptic.name
        let rarity = Optic.compose info CardInfoOptic.rarity
        let typeLine = Optic.compose info CardInfoOptic.typeLine

    [<RequireQualifiedAccess>]
    module EntryWithInfoOptic =
        // Base Optic
        let entry: Lens<EntryWithInfo, Entry> = WithInfo.data
        let info: Lens<EntryWithInfo, CardInfo> = WithInfo.info

        // CardEntry Optic
        let amount = Optic.compose entry EntryOptic.amount
        let foil = Optic.compose entry EntryOptic.foil
        let language = Optic.compose entry EntryOptic.language
        let number = Optic.compose entry EntryOptic.number
        let set = Optic.compose entry EntryOptic.set

        // Info Optic
        let cmc = Optic.compose info CardInfoOptic.cmc
        let colorIdentity = Optic.compose info CardInfoOptic.colorIdentity
        let name = Optic.compose info CardInfoOptic.name
        let rarity = Optic.compose info CardInfoOptic.rarity
        let typeLine = Optic.compose info CardInfoOptic.typeLine

    [<RequireQualifiedAccess>]
    module AgedEntryWithInfoOptic =
        // Base Optic
        let agedEntry: Lens<AgedEntryWithInfo, AgedEntry> = WithInfo.data
        let info: Lens<AgedEntryWithInfo, CardInfo> = WithInfo.info

        // AgedCardEntryOptic Optic
        let amountOld = Optic.compose agedEntry AgedEntryOptic.amountOld
        let amount = Optic.compose agedEntry AgedEntryOptic.amount
        let foil = Optic.compose agedEntry AgedEntryOptic.foil
        let language = Optic.compose agedEntry AgedEntryOptic.language
        let number = Optic.compose agedEntry AgedEntryOptic.number
        let set = Optic.compose agedEntry AgedEntryOptic.set

        // Info Optic
        let cmc = Optic.compose info CardInfoOptic.cmc
        let colorIdentity = Optic.compose info CardInfoOptic.colorIdentity
        let name = Optic.compose info CardInfoOptic.name
        let rarity = Optic.compose info CardInfoOptic.rarity
        let typeLine = Optic.compose info CardInfoOptic.typeLine
