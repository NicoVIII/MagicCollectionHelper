namespace MagicCollectionHelper

open System

module CardData =
    // Data mostly from https://mtg.gamepedia.com/Set#List_of_Magic_expansions_and_sets
    // TODO: Dynamically fetch those data from an API
    let setData =
        Map.empty
        |> Map.add "RAV" ("Ravnica: City of Guilds", 306u, "2005-10-07")
        |> Map.add "TSP" ("Timespiral", 422u, "2006-10-06")
        |> Map.add "ALA" ("Shards of Alara", 249u, "2008-10-03")
        |> Map.add "M11" ("Magic 2011", 249u, "2010-07-16")
        |> Map.add "ISD" ("Innistrad", 264u, "2011-09-30")
        |> Map.add "RTR" ("Return to Ravnica", 274u, "2012-10-05")
        |> Map.add "M13" ("Magic 2013", 249u, "2012-07-13")
        |> Map.add "M14" ("Magic 2014", 249u, "2013-07-19")
        |> Map.add "THS" ("Theros", 249u, "2013-09-27")
        |> Map.add "M15" ("Magic 2015", 269u, "2014-07-18")
        |> Map.add "KTK" ("Khans of Tarkir", 269u, "2014-09-26")
        |> Map.add "OGW" ("Oath of the Gatewatch", 184u, "2016-01-22")
        |> Map.add "EMA" ("Eternal Masters", 249u, "2016-06-10")
        |> Map.add "KLD" ("Kaladesh", 264u, "2016-09-30")
        |> Map.add "AER" ("Aether Revolt", 184u, "2017-01-20")
        |> Map.add "MM3" ("Modern Masters 2017", 249u, "2017-03-17")
        |> Map.add "AKH" ("Amonkhet", 269u, "2017-04-28")
        |> Map.add "XLN" ("Ixalan", 299u, "2017-09-29")
        |> Map.add "RIX" ("Rivals of Ixalan", 196u, "2018-01-19")
        |> Map.add "DOM" ("Dominaria", 269u, "2018-04-27")
        |> Map.add "M19" ("Core Set 2019", 280u, "2018-07-13")
        |> Map.add "GRN" ("Guilds of Ravnica", 259u, "2018-10-05")
        |> Map.add "RNA" ("Ravnica Allegiance", 283u, "2019-01-25")
        |> Map.add "WAR" ("War of the Spark", 264u, "2019-05-03")
        |> Map.add "M20" ("Core Set 2020", 345u, "2019-07-12")
        |> Map.add "ELD" ("Throne of Eldraine", 269u, "2019-10-04")
        |> Map.add "THD" ("Theros Beyond Death", 254u, "2020-01-24")
        |> Map.add "IKO" ("Ikoria: Lair of Behemoths", 254u, "2020-04-17")
        |> Map.add "M21" ("Core Set 2021", 274u, "2020-07-03")
