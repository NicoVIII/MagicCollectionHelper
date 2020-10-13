namespace MagicCollectionHelper.Types

open Argu

/// Available CliArguments
type CliArguments =
    | [<AltCommandLine("-m")>] MissingPercent of float
    | [<MainCommand; ExactlyOnce; Last>] CollectionFile of path: string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | MissingPercent _ -> "how much percent of the collection has to be collected to show missing card ids."
            | CollectionFile _ -> "file to analyse."

/// Config used by the program. Parsed from CliArguments.
type ProgramConfig =
    { filePath: string
      missingPercent: float }
    static member create filePath missingPercent =
        { filePath = filePath
          missingPercent = missingPercent }

// TODO: Provide additional data for set and Language through external file?
// TODO: So a user could add it, if it is missing in the application itself
type MagicSet =
    | CardSet of string
    | TokenSet of string

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

type Analyser<'T, 'S> =
    { emptyData: (unit -> 'S)
      collect: ('S -> CardEntry -> 'S)
      postprocess: ('S -> 'T) }

module Analyser =
    let create emptyData collect postprocess =
        { emptyData = emptyData
          collect = collect
          postprocess = postprocess }
