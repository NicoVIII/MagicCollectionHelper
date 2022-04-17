namespace MagicCollectionHelper.Core

open SimpleOptics

[<AutoOpen>]
module CardTypesLenses =
    module EntryLenses =
        let foil = Optic.compose EntryLenses.card CardLenses.foil
        let language = Optic.compose EntryLenses.card CardLenses.language
        let number = Optic.compose EntryLenses.card CardLenses.number
        let set = Optic.compose EntryLenses.card CardLenses.set

    module OldableLenses =
        let old =
            Lens((fun oldable -> oldable.old), (fun oldable value -> { oldable with old = value }))

        let data =
            Lens(
                (fun (oldable: Oldable<'a>) -> oldable.data),
                (fun oldable value -> { oldable with data = value })
            )

    module OldAmountableLenses =
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

    module WithInfo =
        let data =
            Lens(
                (fun withInfo -> withInfo.data),
                (fun withInfo value -> { withInfo with data = value })
            )

        let info =
            Lens(
                (fun withInfo -> withInfo.info),
                (fun withInfo value -> { withInfo with info = value })
            )

    module AgedCardLenses =
        // Base lenses
        let card: Lens<AgedCard, Card> = OldableLenses.data
        let old: Lens<AgedCard, bool> = OldableLenses.old

        // Card lenses
        let foil = Optic.compose card CardLenses.foil
        let language = Optic.compose card CardLenses.language
        let number = Optic.compose card CardLenses.number
        let set = Optic.compose card CardLenses.set

    module AgedEntryLenses =
        // Base lenses
        let entry: Lens<AgedEntry, Entry> = OldAmountableLenses.data
        let amountOld: Lens<AgedEntry, uint> = OldAmountableLenses.amountOld

        // CardEntry lenses
        let amount = Optic.compose entry EntryLenses.amount
        let card = Optic.compose entry EntryLenses.card
        let foil = Optic.compose entry EntryLenses.foil
        let language = Optic.compose entry EntryLenses.language
        let number = Optic.compose entry EntryLenses.number
        let set = Optic.compose entry EntryLenses.set

    module CardWithInfoLenses =
        // Base lenses
        let card: Lens<CardWithInfo, Card> = WithInfo.data
        let info: Lens<CardWithInfo, CardInfo> = WithInfo.info

        // Card lenses
        let foil = Optic.compose card CardLenses.foil
        let language = Optic.compose card CardLenses.language
        let number = Optic.compose card CardLenses.number
        let set = Optic.compose card CardLenses.set

        // Info lenses
        let cmc = Optic.compose info CardInfoLenses.cmc
        let colorIdentity = Optic.compose info CardInfoLenses.colorIdentity
        let name = Optic.compose info CardInfoLenses.name
        let rarity = Optic.compose info CardInfoLenses.rarity
        let typeLine = Optic.compose info CardInfoLenses.typeLine

    module EntryWithInfoLenses =
        // Base lenses
        let entry: Lens<EntryWithInfo, Entry> = WithInfo.data
        let info: Lens<EntryWithInfo, CardInfo> = WithInfo.info

        // CardEntry lenses
        let amount = Optic.compose entry EntryLenses.amount
        let foil = Optic.compose entry EntryLenses.foil
        let language = Optic.compose entry EntryLenses.language
        let number = Optic.compose entry EntryLenses.number
        let set = Optic.compose entry EntryLenses.set

        // Info lenses
        let cmc = Optic.compose info CardInfoLenses.cmc
        let colorIdentity = Optic.compose info CardInfoLenses.colorIdentity
        let name = Optic.compose info CardInfoLenses.name
        let rarity = Optic.compose info CardInfoLenses.rarity
        let typeLine = Optic.compose info CardInfoLenses.typeLine

    module AgedEntryWithInfoLenses =
        // Base lenses
        let agedEntry: Lens<AgedEntryWithInfo, AgedEntry> = WithInfo.data
        let info: Lens<AgedEntryWithInfo, CardInfo> = WithInfo.info

        // AgedCardEntryLenses lenses
        let amountOld = Optic.compose agedEntry AgedEntryLenses.amountOld
        let amount = Optic.compose agedEntry AgedEntryLenses.amount
        let foil = Optic.compose agedEntry AgedEntryLenses.foil
        let language = Optic.compose agedEntry AgedEntryLenses.language
        let number = Optic.compose agedEntry AgedEntryLenses.number
        let set = Optic.compose agedEntry AgedEntryLenses.set

        // Info lenses
        let cmc = Optic.compose info CardInfoLenses.cmc
        let colorIdentity = Optic.compose info CardInfoLenses.colorIdentity
        let name = Optic.compose info CardInfoLenses.name
        let rarity = Optic.compose info CardInfoLenses.rarity
        let typeLine = Optic.compose info CardInfoLenses.typeLine
