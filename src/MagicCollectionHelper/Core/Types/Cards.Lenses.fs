namespace MagicCollectionHelper.Core

[<AutoOpen>]
module CardTypesLenses =
    module EntryLenses =
        let foil = EntryLenses.card << CardLenses.foil
        let language = EntryLenses.card << CardLenses.language
        let number = EntryLenses.card << CardLenses.number
        let set = EntryLenses.card << CardLenses.set

    module OldableLenses =
        let old =
            Lens((fun oldable -> oldable.old), (fun oldable value -> { oldable with old = value }))

        let data =
            Lens((fun (oldable: Oldable<'a>) -> oldable.data), (fun oldable value -> { oldable with data = value }))

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
            Lens((fun withInfo -> withInfo.data), (fun withInfo value -> { withInfo with data = value }))

        let info =
            Lens((fun withInfo -> withInfo.info), (fun withInfo value -> { withInfo with info = value }))

    module AgedCardLenses =
        // Base lenses
        let card : Lens<AgedCard, Card> = OldableLenses.data
        let old : Lens<AgedCard, bool> = OldableLenses.old

        // Card lenses
        let foil = card << CardLenses.foil
        let language = card << CardLenses.language
        let number = card << CardLenses.number
        let set = card << CardLenses.set

    module AgedEntryLenses =
        // Base lenses
        let entry : Lens<AgedEntry, Entry> = OldAmountableLenses.data
        let amountOld : Lens<AgedEntry, uint> = OldAmountableLenses.amountOld

        // CardEntry lenses
        let amount = entry << EntryLenses.amount
        let card = entry << EntryLenses.card
        let foil = entry << EntryLenses.foil
        let language = entry << EntryLenses.language
        let number = entry << EntryLenses.number
        let set = entry << EntryLenses.set

    module CardWithInfoLenses =
        // Base lenses
        let card : Lens<CardWithInfo, Card> = WithInfo.data
        let info : Lens<CardWithInfo, CardInfo> = WithInfo.info

        // Card lenses
        let foil = card << CardLenses.foil
        let language = card << CardLenses.language
        let number = card << CardLenses.number
        let set = card << CardLenses.set

        // Info lenses
        let cmc = info << CardInfoLenses.cmc
        let colorIdentity = info << CardInfoLenses.colorIdentity
        let name = info << CardInfoLenses.name
        let rarity = info << CardInfoLenses.rarity
        let typeLine = info << CardInfoLenses.typeLine

    module EntryWithInfoLenses =
        // Base lenses
        let entry : Lens<EntryWithInfo, Entry> = WithInfo.data
        let info : Lens<EntryWithInfo, CardInfo> = WithInfo.info

        // CardEntry lenses
        let amount = entry << EntryLenses.amount
        let foil = entry << EntryLenses.foil
        let language = entry << EntryLenses.language
        let number = entry << EntryLenses.number
        let set = entry << EntryLenses.set

        // Info lenses
        let cmc = info << CardInfoLenses.cmc
        let colorIdentity = info << CardInfoLenses.colorIdentity
        let name = info << CardInfoLenses.name
        let rarity = info << CardInfoLenses.rarity
        let typeLine = info << CardInfoLenses.typeLine

    module AgedEntryWithInfoLenses =
        // Base lenses
        let agedEntry : Lens<AgedEntryWithInfo, AgedEntry> = WithInfo.data
        let info : Lens<AgedEntryWithInfo, CardInfo> = WithInfo.info

        // AgedCardEntryLenses lenses
        let amountOld = agedEntry << AgedEntryLenses.amountOld
        let amount = agedEntry << AgedEntryLenses.amount
        let foil = agedEntry << AgedEntryLenses.foil
        let language = agedEntry << AgedEntryLenses.language
        let number = agedEntry << AgedEntryLenses.number
        let set = agedEntry << AgedEntryLenses.set

        // Info lenses
        let cmc = info << CardInfoLenses.cmc
        let colorIdentity = info << CardInfoLenses.colorIdentity
        let name = info << CardInfoLenses.name
        let rarity = info << CardInfoLenses.rarity
        let typeLine = info << CardInfoLenses.typeLine
