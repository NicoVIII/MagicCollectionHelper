namespace MagicCollectionHelper.Core

module String =
    /// Case insensitive version of Contains
    let iContains (haystack: string) (needle: string) =
        (haystack.ToLower()).Contains(needle.ToLower())

    let startsWith (needle: string) (haystack: string) = haystack.StartsWith needle
