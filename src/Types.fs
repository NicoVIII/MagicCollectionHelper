namespace MagicCollectionHelper.Types

type Arguments = { filePath: string }

/// Includes every known set of MTG
// TODO: Provide additional data for set and Language through external file?
// TODO: So a user could add it, if it is missing in the application itself
type MagicSet = | Core2020

type Language =
    | English
    | German

// TODO: condition
// TODO: comment?
// TODO: pinned?
// TODO: added
type CardEntry =
    { amount: uint
      name: string
      number: string // TODO: typesafety
      foil: bool
      language: Language
      set: MagicSet }

type Analyzer<'T> =
    { emptyData: (unit -> 'T)
      analyze: ('T -> CardEntry -> 'T) }

module Analyzer =
    let create emptyData analyze =
        { emptyData = emptyData
          analyze = analyze }
