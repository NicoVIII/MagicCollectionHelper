namespace MagicCollectionHelper.Types

type Arguments = { filePath: string }

// TODO: Provide additional data for set and Language through external file?
// TODO: So a user could add it, if it is missing in the application itself
type MagicSet = MagicSet of string

type Language = Language of string

// TODO: condition
// TODO: comment?
// TODO: pinned?
// TODO: added
type CardEntry =
    { amount: uint
      name: string
      number: uint option
      foil: bool
      language: Language option
      set: MagicSet option }

type Analyzer<'T, 'S> =
    { emptyData: (unit -> 'S)
      collect: ('S -> CardEntry -> 'S)
      postprocess: ('S -> 'T) }

module Analyzer =
    let create emptyData collect postprocess =
        { emptyData = emptyData
          collect = collect
          postprocess = postprocess }
