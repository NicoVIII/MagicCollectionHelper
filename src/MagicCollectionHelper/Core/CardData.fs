namespace MagicCollectionHelper.Core

open System

open MagicCollectionHelper.Core.Types

module CardData =
    let private add (set, max, date: string, tokenMax, name: string) : SetDataMap -> SetDataMap =
        let addCardSet =
            Map.add (MagicSet.create set) { date = date; max = max; name = name }

        let addTokenSet =
            match tokenMax with
            | Some max ->
                Map.add
                    (MagicSet.create $"T{set}")
                    { date = date
                      max = max
                      name = $"{name} Token" }
            | None -> id

        addCardSet >> addTokenSet

    // Data mostly from https://mtg.gamepedia.com/Set#List_of_Magic_expansions_and_sets
    // TODO: Dynamically fetch those data from an API or an editable config file
    let createSetData () =
        Map.empty
        |> add ("LEA", 295u, "1993-08-05", None, "Limited Edition Alpha")
        |> add ("LEG", 310u, "1994-06-10", None, "Legends")
        |> add ("FEM", 102u, "1994-11-15", None, "Fallen Empires")
        |> add ("4ED", 378u, "1995-04-01", None, "Fourth Edition")
        |> add ("ICE", 383u, "1995-06-01", None, "Ice Age")
        |> add ("ALL", 144u, "1996-06-10", None, "Alliances")
        |> add ("MIR", 350u, "1996-10-08", None, "Mirage")
        |> add ("5ED", 449u, "1997-03-24", None, "Fifth Edition")
        |> add ("TMP", 350u, "1997-10-14", None, "Tempest")
        |> add ("INV", 350u, "2000-10-02", None, "Invasion")
        |> add ("APC", 143u, "2001-06-04", Some 1u, "Apocalypse")
        |> add ("ODY", 350u, "2001-10-01", None, "Odyssey")
        |> add ("MRD", 306u, "2003-10-02", None, "Mirrodin")
        |> add ("8ED", 357u, "2003-07-28", None, "Eigth Edition")
        |> add ("5DN", 165u, "2004-06-04", Some 2u, "Fifth Dawn")
        |> add ("CHK", 306u, "2004-10-01", None, "Champions of Kamigawa")
        |> add ("BOK", 165u, "2005-02-04", None, "Betrayers of Kamigawa")
        |> add ("SOK", 165u, "2005-06-03", None, "Saviours of Kamigawa")
        |> add ("RAV", 306u, "2005-10-07", None, "Ravnica: City of Guilds")
        |> add ("GPT", 165u, "2006-02-03", None, "Guildpact")
        |> add ("DIS", 180u, "2006-05-05", None, "Dissension")
        |> add ("CSP", 155u, "2006-07-21", None, "Coldsnap")
        |> add ("TSP", 301u, "2006-10-06", None, "Timespiral")
        |> add ("TSB", 121u, "2006-10-06", None, "Timespiral Timeshifted")
        |> add ("PLC", 165u, "2007-02-02", None, "Planar Chaos")
        |> add ("FUT", 180u, "2007-05-04", None, "Future Sigth")
        |> add ("10E", 383u, "2007-07-13", Some 6u, "Tenth Edition")
        |> add ("EVE", 180u, "2008-07-25", Some 7u, "Eventide")
        |> add ("ALA", 249u, "2008-10-03", Some 10u, "Shards of Alara")
        |> add ("ARB", 145u, "2009-04-30", Some 4u, "Alara Reborn")
        |> add ("M10", 249u, "2009-07-17", Some 8u, "Magic 2010")
        |> add ("ZEN", 249u, "2009-10-02", Some 11u, "Zendikar")
        |> add ("M11", 249u, "2010-07-16", Some 6u, "Magic 2011")
        |> add ("SOM", 249u, "2010-10-01", Some 9u, "Scars of Mirrodin")
        |> add ("MBS", 155u, "2011-02-04", Some 5u, "Mirrodin Besieged")
        |> add ("NPH", 175u, "2011-05-13", Some 4u, "New Phyrexia")
        |> add ("M12", 249u, "2011-07-15", Some 7u, "Magic 2012")
        |> add ("ISD", 264u, "2011-09-30", Some 12u, "Innistrad")
        |> add ("DKA", 158u, "2012-02-03", Some 3u, "Dark Ascension")
        |> add ("AVR", 244u, "2012-05-04", Some 8u, "Avacyn Restored")
        |> add ("PC2", 156u, "2012-06-01", None, "Planechase 2012")
        |> add ("M13", 249u, "2012-07-13", Some 11u, "Magic 2013")
        |> add ("RTR", 274u, "2012-10-05", Some 12u, "Return to Ravnica")
        |> add ("GTC", 249u, "2013-02-01", Some 8u, "Gatecrash")
        |> add ("M14", 249u, "2013-07-19", Some 13u, "Magic 2014")
        |> add ("THS", 249u, "2013-09-27", Some 11u, "Theros")
        |> add ("BNG", 165u, "2014-02-07", Some 11u, "Born of the Gods")
        |> add ("JOU", 165u, "2014-05-02", Some 6u, "Journey into Nyx")
        |> add ("M15", 269u, "2014-07-18", Some 14u, "Magic 2015")
        |> add ("KTK", 269u, "2014-09-26", Some 13u, "Khans of Tarkir")
        |> add ("DTK", 264u, "2015-03-27", Some 8u, "Dragons of Tarkir")
        |> add ("ORI", 272u, "2015-07-17", Some 14u, "Magic Origins")
        |> add ("BFZ", 274u, "2015-10-02", Some 14u, "Battle for Zendikar")
        |> add ("OGW", 184u, "2016-01-22", Some 11u, "Oath of the Gatewatch")
        |> add ("SOI", 297u, "2016-04-08", Some 18u, "Shadows over Innistrad")
        |> add ("EMA", 249u, "2016-06-10", Some 16u, "Eternal Masters")
        |> add ("EMN", 205u, "2016-07-22", Some 10u, "Eldritch Moon")
        |> add ("CN2", 221u, "2016-08-26", Some 12u, "Conspiracy: Take the Crown")
        |> add ("KLD", 264u, "2016-09-30", Some 12u, "Kaladesh")
        |> add ("AER", 184u, "2017-01-20", Some 4u, "Aether Revolt")
        |> add ("MM3", 249u, "2017-03-17", Some 21u, "Modern Masters 2017")
        |> add ("AKH", 269u, "2017-04-28", Some 25u, "Amonkhet")
        |> add ("E01", 106u, "2017-06-16", None, "Archenemy: Nicol Bolas")
        |> add ("HOU", 199u, "2017-07-14", Some 12u, "Hour of Devastation")
        |> add ("C17", 309u, "2017-08-25", Some 11u, "Commander 2017")
        |> add ("XLN", 299u, "2017-09-29", Some 10u, "Ixalan")
        |> add ("RIX", 196u, "2018-01-19", Some 6u, "Rivals of Ixalan")
        |> add ("DOM", 269u, "2018-04-27", Some 16u, "Dominaria")
        |> add ("M19", 280u, "2018-07-13", Some 17u, "Core Set 2019")
        |> add ("GRN", 259u, "2018-10-05", Some 8u, "Guilds of Ravnica")
        |> add ("RNA", 259u, "2019-01-25", Some 13u, "Ravnica Allegiance")
        |> add ("WAR", 264u, "2019-05-03", Some 19u, "War of the Spark")
        |> add ("M20", 345u, "2019-07-12", Some 12u, "Core Set 2020")
        |> add ("ELD", 269u, "2019-10-04", Some 20u, "Throne of Eldraine")
        |> add ("MB1", 1697u, "2019-11-07", None, "Mystery Booster")
        |> add ("THB", 254u, "2020-01-24", Some 14u, "Theros Beyond Death")
        |> add ("IKO", 254u, "2020-04-17", Some 13u, "Ikoria: Lair of Behemoths")
        |> add ("M21", 274u, "2020-07-03", Some 18u, "Core Set 2021")
        |> add ("2XM", 332u, "2020-08-07", Some 31u, "Double Masters")
        |> add ("ZNR", 280u, "2020-09-25", Some 12u, "Zendikar Rising")
        |> add ("CMR", 361u, "2020-11-20", Some 14u, "Commander Legends")
        |> add ("KHM", 285u, "2021-02-05", Some 23u, "Kaldheim")
