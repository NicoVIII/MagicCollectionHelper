namespace MagicCollectionHelper.Core

[<AutoOpen>]
module DomainTypes =
    type NumBase =
        | Decimal
        | Dozenal
        | Seximal

    /// Type which holds user preferences so the user can customize some behaviors
    type Prefs =
        {
            cardGroupMinSize: uint
            cardGroupMaxSize: uint
            numBase: NumBase
            missingPercent: float
            setWithFoils: bool
        }

    // TODO: condition
    // TODO: comment?
    // TODO: pinned?
    // TODO: added
    type DeckStatsCardEntry =
        {
            amount: uint
            name: string
            number: CollectorNumber option
            foil: bool
            language: Language option
            set: MagicSet option
        }

    type SetData =
        {
            date: string
            max: uint
            name: string
        }

    type SetDataMap = Map<MagicSet, SetData>

    type Analyser<'result, 'collect, 'settings> =
        {
            emptyData: (unit -> 'collect)
            collect: ('settings -> 'collect -> DeckStatsCardEntry -> 'collect)
            postprocess: (SetDataMap -> 'collect -> 'result)
            print: ('settings -> 'result -> string seq)
        }

    type SortRule =
        | ByColorIdentity
        | BySet
        | ByCollectorNumber
        | ByName
        | ByCmc
        | ByTypeContains of string list
        | ByRarity of Set<Rarity> list
        | ByLanguage of Language list

    type SortRules = SortRule list

    type CustomLocationName = string

    type Rules =
        {
            inSet: Set<MagicSet> option
            inLanguage: Language option
            isFoil: bool option
            isToken: bool option
            typeContains: Set<string> option
            typeNotContains: Set<string> option
            limit: uint option
            limitExact: uint option
            rarity: Set<Rarity> option
            colorIdentity: Set<ColorIdentity> option
        }

    type CustomLocation =
        {
            name: CustomLocationName
            rules: Rules
            sortBy: SortRules
        }

    /// Location, where a part of the collection is
    type InventoryLocation =
        | Custom of CustomLocation
        | Fallback

    type LocationWithCards = (InventoryLocation * AgedEntry list) list
