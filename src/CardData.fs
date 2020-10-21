namespace MagicCollectionHelper

open System

open MagicCollectionHelper.Types

module CardData =
    let private add (set, max, date: string, tokenMax, name: string): SetDataMap -> SetDataMap =
        // Wrap data in value objects
        let set = set |> MagicSet

        Map.add
            set
            { date = date
              maxCard = max |> CardNumber
              maxToken = Option.map (TokenNumber) tokenMax
              name = name }

    // Data mostly from https://mtg.gamepedia.com/Set#List_of_Magic_expansions_and_sets
    // TODO: Dynamically fetch those data from an API or an editable config file
    let createSetData () =
        Map.empty
        |> add ("INV", 350u, "2000-10-02", None, "Invasion")
        |> add ("ODY", 350u, "2001-10-01", None, "Odyssey")
        |> add ("RAV", 306u, "2005-10-07", None, "Ravnica: City of Guilds")
        |> add ("TSP", 422u, "2006-10-06", None, "Timespiral")
        |> add ("ALA", 249u, "2008-10-03", Some 10u, "Shards of Alara")
        |> add ("ARB", 145u, "2009-04-30", Some 4u, "Alara Reborn")
        |> add ("M10", 249u, "2009-07-17", Some 8u, "Magic 2010")
        |> add ("M11", 249u, "2010-07-16", Some 6u, "Magic 2011")
        |> add ("SOM", 249u, "2010-10-01", Some 9u, "Scars of Mirrodin")
        |> add ("M12", 249u, "2011-07-15", Some 7u, "Magic 2012")
        |> add ("ISD", 264u, "2011-09-30", Some 12u, "Innistrad")
        |> add ("AVR", 244u, "2012-05-04", Some 8u, "Avacyn Restored")
        |> add ("M13", 249u, "2012-07-13", Some 11u, "Magic 2013")
        |> add ("RTR", 274u, "2012-10-05", Some 12u, "Return to Ravnica")
        |> add ("M14", 249u, "2013-07-19", Some 13u, "Magic 2014")
        |> add ("THS", 249u, "2013-09-27", Some 11u, "Theros")
        |> add ("JOU", 165u, "2014-05-02", Some 6u, "Journey into Nyx")
        |> add ("M15", 269u, "2014-07-18", Some 14u, "Magic 2015")
        |> add ("KTK", 269u, "2014-09-26", Some 13u, "Khans of Tarkir")
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
        |> add ("HOU", 199u, "2017-07-14", Some 12u, "Hour of Devastation")
        |> add ("XLN", 299u, "2017-09-29", Some 10u, "Ixalan")
        |> add ("RIX", 196u, "2018-01-19", Some 6u, "Rivals of Ixalan")
        |> add ("DOM", 269u, "2018-04-27", Some 16u, "Dominaria")
        |> add ("M19", 280u, "2018-07-13", Some 17u, "Core Set 2019")
        |> add ("GRN", 259u, "2018-10-05", Some 8u, "Guilds of Ravnica")
        |> add ("RNA", 259u, "2019-01-25", Some 13u, "Ravnica Allegiance")
        |> add ("WAR", 264u, "2019-05-03", Some 19u, "War of the Spark")
        |> add ("M20", 345u, "2019-07-12", Some 12u, "Core Set 2020")
        |> add ("ELD", 269u, "2019-10-04", Some 20u, "Throne of Eldraine")
        |> add ("THB", 254u, "2020-01-24", Some 14u, "Theros Beyond Death")
        |> add ("IKO", 254u, "2020-04-17", Some 13u, "Ikoria: Lair of Behemoths")
        |> add ("M21", 274u, "2020-07-03", Some 18u, "Core Set 2021")
        |> add ("2XM", 332u, "2020-08-07", Some 31u, "Double Masters")
        |> add ("ZNR", 280u, "2020-09-25", Some 12u, "Zendikar Rising")
