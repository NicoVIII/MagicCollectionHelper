namespace MagicCollectionHelper.Core

[<AutoOpen>]
module CardTypesModules =
    module CollectorNumber =
        let fromString (s: string) =
            // We remove trailing zeros from the string
            s.TrimStart('0') |> CollectorNumber

    module MagicSet =
        /// Converts some old two letter set abbreviations to the new three letter counterpart
        /// New abbreviatens taken from https://mtg.gamepedia.com/Set#List_of_Magic_expansions_and_sets
        let convertSetAbbrev =
            function
            | "1E" -> "LEA" // Alpha (Limited Edition)
            | "2E" -> "LEB" // Beta (Limited Edition)
            | "2U" -> "2ED" // Unlimited Edition
            | "AN" -> "ARN" // Arabian Nights
            | "AQ" -> "ATQ" // Antiquities
            | "3E" -> "3ED" // Revised Edition
            | "LE" -> "LEG" // Legends
            | "DK" -> "DRK" // The Dark
            | "FE" -> "FEM" // Fallen Empires
            | "4E" -> "4ED" // Fourth Edition
            | "IA" -> "ICE" // Ice Age
            | "CH" -> "CHR" // Chronicles
            | "HM" -> "HML" // Homelands
            | "AL" -> "ALL" // Alliances
            | "MI" -> "MIR" // Mirage
            | "VI" -> "VIS" // Visions
            | "5E" -> "5ED" // Fifth Edition
            | "PO" -> "POR" // Portal
            | "WL" -> "WTH" // Weatherlight
            | "TE" -> "TMP" // Tempest
            | "ST" -> "STH" // Stronghold
            | "EX" -> "EXO" // Exodus
            | "P2" -> "P02" // Portal Second Age
            | "UG" -> "UGL" // Unglued
            | "UZ" -> "USG" // Urza's Saga
            | "UL" -> "ULG" // Urza's Legacy
            | "6E" -> "6ED" // Sixth Edition
            | "PK" -> "PTK" // Portal Three Kingdoms
            | "UD" -> "UDS" // Urza's Destiny
            | "P3" -> "S99" // Starter 1999
            | "MM" -> "MMQ" // Mercadian Masques
            | "NE" -> "NEM" // Nemesis
            | "PR" -> "PCY" // Prophecy
            | "IN" -> "INV" // Invasion
            | "PS" -> "PLS" // Planeshift
            | "7E" -> "7ED" // Seventh Edition
            | "AP" -> "APC" // Apocalypse
            | "OD" -> "ODY" // Odyssey
            // Deckstats has strange abbreviations I fix here
            | "GU" -> "ULG" // Urza's Legacy
            | "10ED" -> "10E" // Tenth Edition
            | set -> set

        let create =
            (fun (s: string) -> s.ToUpper())
            >> convertSetAbbrev
            >> MagicSet

    module ColorIdentity =
        let colorless = Set.empty

        // Mono
        let white = White |> Set.singleton
        let blue = Blue |> Set.singleton
        let black = Black |> Set.singleton
        let red = Red |> Set.singleton
        let green = Green |> Set.singleton

        // Two colors - Ravnica guilds
        let azorius = [ White; Blue ] |> Set.ofList
        let dimir = [ Blue; Black ] |> Set.ofList
        let rakdos = [ Black; Red ] |> Set.ofList
        let gruul = [ Red; Green ] |> Set.ofList
        let selesnya = [ Green; White ] |> Set.ofList
        let orzhov = [ White; Black ] |> Set.ofList
        let izzet = [ Blue; Red ] |> Set.ofList
        let golgari = [ Black; Green ] |> Set.ofList
        let boros = [ Red; White ] |> Set.ofList
        let simic = [ Green; Blue ] |> Set.ofList

        // Three colors - shards of alara + div.
        let bant = [ Green; White; Blue ] |> Set.ofList
        let esper = [ White; Blue; Black ] |> Set.ofList
        let grixis = [ Blue; Black; Red ] |> Set.ofList
        let jund = [ Red; Green; Black ] |> Set.ofList
        let naya = [ Red; Green; White ] |> Set.ofList
        let abzan = [ White; Black; Green ] |> Set.ofList
        let jeskai = [ Blue; Red; White ] |> Set.ofList
        let sultai = [ Black; Green; Blue ] |> Set.ofList
        let mardu = [ Red; White; Black ] |> Set.ofList
        let temur = [ Green; Blue; Red ] |> Set.ofList

        // Four colors - shards of alara + div.
        let nonWhite =
            [ Blue; Black; Red; Green ] |> Set.ofList

        let nonBlue =
            [ Black; Red; Green; White ] |> Set.ofList

        let nonBlack =
            [ Red; Green; White; Blue ] |> Set.ofList

        let nonRed =
            [ Green; White; Blue; Black ] |> Set.ofList

        let nonGreen =
            [ White; Blue; Black; Red ] |> Set.ofList

        // Five colors
        let allColors =
            [ White; Blue; Black; Red; Green ] |> Set.ofList

        let private sorted =
            [ colorless
              white
              blue
              black
              red
              green
              azorius
              dimir
              rakdos
              gruul
              selesnya
              orzhov
              izzet
              golgari
              boros
              simic
              bant
              esper
              grixis
              jund
              naya
              abzan
              jeskai
              sultai
              mardu
              temur
              nonWhite
              nonBlue
              nonBlack
              nonRed
              nonGreen
              allColors ]

        let toString (ci: ColorIdentity) =
            match ci with
            | _ when ci = colorless -> "Colorless"
            | _ when ci = white -> "W"
            | _ when ci = blue -> "U"
            | _ when ci = black -> "B"
            | _ when ci = red -> "R"
            | _ when ci = green -> "G"
            | _ when ci = azorius -> "WU"
            | _ when ci = dimir -> "UB"
            | _ when ci = rakdos -> "BR"
            | _ when ci = gruul -> "RG"
            | _ when ci = selesnya -> "GW"
            | _ when ci = orzhov -> "WB"
            | _ when ci = izzet -> "UR"
            | _ when ci = golgari -> "BG"
            | _ when ci = boros -> "RW"
            | _ when ci = simic -> "GU"
            | _ when ci = bant -> "GWU"
            | _ when ci = esper -> "WUG"
            | _ when ci = grixis -> "UBR"
            | _ when ci = jund -> "BRG"
            | _ when ci = naya -> "RGW"
            | _ when ci = abzan -> "WBG"
            | _ when ci = jeskai -> "URW"
            | _ when ci = sultai -> "BGU"
            | _ when ci = mardu -> "RWB"
            | _ when ci = temur -> "GUR"
            | _ when ci = nonWhite -> "UBRG"
            | _ when ci = nonBlue -> "BRGW"
            | _ when ci = nonBlack -> "GWUR"
            | _ when ci = nonRed -> "RGWU"
            | _ when ci = nonGreen -> "WUBR"
            | _ when ci = allColors -> "Five color"
            | _ -> failwith "Boom"

        let private posMap =
            sorted
            |> List.indexed
            |> List.map (fun (index, colorIdentity) -> (colorIdentity, index))
            |> Map.ofList

        let getPosition (colorIdentity: ColorIdentity) = posMap.Item colorIdentity

    module Card =
        let isExactSame (card1: Card) (card2: Card) =
            card1.set = card2.set
            && card1.number = card2.number

        let isToken (card: Card) =
            let setValue = card.set.Value
            setValue.Length = 4 && setValue.StartsWith "T"

    module CardEntry =
        /// Takes a list of cards and creates entry out of equal cards
        let collapseCardList cardList =
            cardList
            |> List.fold
                (fun cardAmountMap card ->
                    Map.change
                        card
                        (fun mapEntry ->
                            mapEntry
                            |> Option.defaultValue 0u
                            |> (+) 1u
                            |> Some)
                        cardAmountMap)
                Map.empty
            |> Map.toList
            |> List.map (fun (card, amount) -> { card = card; amount = amount })

    module Oldable =
        let create old data = { old = old; data = data }

        let data (wrapper: Oldable<'a>) = wrapper.data

        let map mapper wrapper =
            create wrapper.old (mapper wrapper.data)

    module OldAmountable =
        let create amountOld data : OldAmountable<'a> = { amountOld = amountOld; data = data }

        let data (wrapper: OldAmountable<'a>) = wrapper.data

        let inline map mapper (wrapper: OldAmountable<'a>) : OldAmountable<'b> =
            create wrapper.amountOld (mapper wrapper.data)

    module WithInfo =
        let create info data = { data = data; info = info }

        let data (withInfo: WithInfo<'a>) = withInfo.data

        let inline map mapper (wrapper: WithInfo<'a>) : WithInfo<'b> =
            create wrapper.info (mapper wrapper.data)

        /// Checks if those two cards are rulewise the same. They do not have to be from the same set
        let isSame withInfo1 withInfo2 =
            withInfo1.info.oracleId = withInfo2.info.oracleId

    module CardWithInfo =
        let create card info : CardWithInfo = { data = card; info = info }

        /// Checks if those two cards are rulewise the same. They do not have to be from the same set
        let isSame (card1: CardWithInfo) (card2: CardWithInfo) = WithInfo.isSame card1 card2

        let isExactSame (card1: CardWithInfo) (card2: CardWithInfo) = Card.isExactSame card1.data card2.data

    module CardEntryWithInfo =
        let create entry info : CardEntryWithInfo = { data = entry; info = info }

    module AgedCard =
        let create old card : AgedCard = Oldable.create old card

    module AgedCardEntry =
        let create amountOld entry : AgedCardEntry = OldAmountable.create amountOld entry

        let determineCardAge oldList newList =
            // We first create a map
            let oldMap =
                oldList
                |> List.map (fun (entry: CardEntry) -> entry.card, entry)
                |> Map.ofList

            newList
            |> List.map
                (fun (entry: CardEntry) ->
                    let amountOld =
                        match Map.tryFind entry.card oldMap with
                        | Some entry -> entry.amount
                        | None -> 0u

                    OldAmountable.create amountOld entry)

    module AgedCardWithInfo =
        let create info card : AgedCardWithInfo = WithInfo.create info card

    module AgedCardEntryWithInfo =
        let create info entry : AgedCardEntryWithInfo = WithInfo.create info entry

        /// Takes a list of cards and creates entry out of equal cards
        let fromCardList (cardList: AgedCardWithInfo list) =
            cardList
            |> List.fold
                (fun cardAmountMap card ->
                    Map.change
                        (WithInfo.map Oldable.data card)
                        (fun mapEntry ->
                            mapEntry
                            |> Option.defaultValue (0u, 0u)
                            |> (fun (a, b) -> a + 1u, b + if card.data.old then 1u else 0u)
                            |> Some)
                        cardAmountMap)
                Map.empty
            |> Map.toList
            |> List.map
                (fun (cardWithInfo: CardWithInfo, (amount, amountOld)) ->
                    cardWithInfo.data
                    |> CardEntry.create amount
                    |> AgedCardEntry.create amountOld
                    |> create cardWithInfo.info)
