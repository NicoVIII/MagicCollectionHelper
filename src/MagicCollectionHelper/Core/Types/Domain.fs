namespace MagicCollectionHelper.Core.Types

open Myriad.Plugins

[<Generator.Lenses("types", "Lens")>]
type Prefs =
    { dozenalize: bool
      missingPercent: float
      setWithFoils: bool }

module Prefs =
    let create dozenalize missingPercent setWithFoils =
        { dozenalize = dozenalize
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

type Card =
    { name: string
      number: SetNumber option
      foil: bool
      language: Language option
      set: MagicSet option }

type Rule =
    | InSet of MagicSet list
    | InLanguage of Language
    | IsFoil of bool
    | Limit of uint

type CustomLocation =
    { name: string
      rules: Rule list }

/// Location, where a part of the collection is
type InventoryLocation =
    | Custom of CustomLocation
    | Fallback
