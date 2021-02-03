namespace MagicCollectionHelper.Types

open Argu

/// Available CliArguments
type CliArguments =
    | Dozenalize
    | SetWithFoils
    | [<AltCommandLine("-m")>] MissingPercent of float
    | [<MainCommand; ExactlyOnce; Last>] CollectionFile of path: string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Dozenalize _ -> "should the numbers be printed in base 12 instead of base 10"
            | SetWithFoils -> "includes foils in set analyser"
            | MissingPercent _ -> "how much percent of the collection has to be collected to show missing card ids."
            | CollectionFile _ -> "file to analyse."

/// Config used by the program. Parsed from CliArguments.
type ProgramConfig =
    { dozenalize: bool
      filePath: string
      missingPercent: float
      setWithFoils: bool }
    static member create dozenalize filePath missingPercent setWithFoils =
        { dozenalize = dozenalize
          filePath = filePath
          missingPercent = missingPercent
          setWithFoils = setWithFoils }

type CardNumber =
    | CardNumber of uint32
    static member unwrap(CardNumber x) = x
    member this.Value = CardNumber.unwrap this

type TokenNumber =
    | TokenNumber of uint32
    static member unwrap(TokenNumber x) = x
    member this.Value = TokenNumber.unwrap this

type SetNumber =
    | SetCardNumber of CardNumber
    | SetTokenNumber of TokenNumber

module SetNumber =
    let Card = CardNumber >> SetCardNumber
    let Token = TokenNumber >> SetTokenNumber

    let splitSet =
        Set.fold
            (fun (cards, tokens) number ->
                match number with
                | SetCardNumber cardNumber -> (cards |> Set.add cardNumber, tokens)
                | SetTokenNumber tokenNumber -> (cards, tokens |> Set.add tokenNumber))
            (Set.empty, Set.empty)

    let splitSeq s =
        Seq.fold
            (fun (cards, tokens) number ->
                match number with
                | SetCardNumber cardNumber -> (Seq.append cards [ cardNumber ], tokens)
                | SetTokenNumber tokenNumber -> (cards, Seq.append tokens [ tokenNumber ]))
            (Seq.empty, Seq.empty)
            s

// TODO: Provide additional data for set and Language through external file?
// TODO: So a user could add it, if it is missing in the application itself
type MagicSet =
    | MagicSet of string
    member this.Value =
        let (MagicSet value) = this
        value

type Language = Language of string

// TODO: condition
// TODO: comment?
// TODO: pinned?
// TODO: added
type CardEntry =
    { amount: uint
      name: string
      number: SetNumber option
      foil: bool
      language: Language option
      set: MagicSet option }

type SetData =
    { date: string
      maxCard: CardNumber
      maxToken: TokenNumber option
      name: string }

type SetDataMap = Map<MagicSet, SetData>

type Analyser<'result, 'collect, 'settings> =
    { emptyData: (unit -> 'collect)
      collect: ('settings -> 'collect -> CardEntry -> 'collect)
      postprocess: (SetDataMap -> 'collect -> 'result)
      print: ('settings -> 'result -> string seq) }

module Analyser =
    let create emptyData collect postprocess print =
        { emptyData = emptyData
          collect = collect
          postprocess = postprocess
          print = print }
