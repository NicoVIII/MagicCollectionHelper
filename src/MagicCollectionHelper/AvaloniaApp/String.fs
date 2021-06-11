namespace MagicCollectionHelper.AvaloniaApp

module String =
    let iContains (haystack: string) (needle: string) =
        (haystack.ToLower()).Contains(needle.ToLower())
