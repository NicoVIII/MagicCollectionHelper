namespace MagicCollectionHelper.Core

[<AutoOpen>]
module CardPartTypesModules =
    module Language =
        let unwrap (Language l) = l

    module CollectorNumber =
        let fromString (s: string) =
            // We remove trailing zeros from the string
            s.TrimStart('0') |> CollectorNumber

        let unwrap (CollectorNumber n) = n

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

        let unwrap (MagicSet v) = v

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
        let nonWhite = [ Blue; Black; Red; Green ] |> Set.ofList

        let nonBlue = [ Black; Red; Green; White ] |> Set.ofList

        let nonBlack = [ Red; Green; White; Blue ] |> Set.ofList

        let nonRed = [ Green; White; Blue; Black ] |> Set.ofList

        let nonGreen = [ White; Blue; Black; Red ] |> Set.ofList

        // Five colors
        let allColors = [ White; Blue; Black; Red; Green ] |> Set.ofList

        let private sorted =
            [
                colorless
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
                allColors
            ]

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
