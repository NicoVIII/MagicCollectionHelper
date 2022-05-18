namespace MagicCollectionHelper.Core

[<AutoOpen>]
module CardPartTypes =
    /// All colors which are important in Magic
    type Color =
        | White
        | Blue
        | Black
        | Red
        | Green

    /// The identifier for a language, typically the language a card is in
    type Language = Language of string

    /// The number for collectors of a card inside a given set
    type CollectorNumber = CollectorNumber of string

    /// The identifier of a magic set
    type MagicSet = MagicSet of string

    type ColorIdentity = Set<Color>

    type Rarity =
        | Common
        | Uncommon
        | Rare
        | Mythic
        | Special
        | Bonus
