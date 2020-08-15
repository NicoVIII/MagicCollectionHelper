namespace MagicCollectionHelper

open System

module CardData =
    // TODO: Dynamically fetch those data from an API
    let setData =
        Map.empty
        |> Map.add "RTR" ("Return to Ravnica", 274u, "2012-10-05")
        |> Map.add "M13" ("Magic 2013", 249u, "2012-07-13")
        |> Map.add "AKH" ("Amonkhet", 269u, "2017-04-28")
        |> Map.add "DOM" ("Dominaria", 269u, "2018-04-27")
        |> Map.add "M19" ("Core Set 2019", 280u, "2018-07-13")
        |> Map.add "M20" ("Core Set 2020", 345u, "2019-07-12")
        |> Map.add "XLN" ("Ixalan", 299u, "2017-09-29")
        |> Map.add "RIX" ("Rivals of Ixalan", 196u, "2018-01-19")
        |> Map.add "GRN" ("Guilds of Ravnica", 259u, "2018-10-05")
        |> Map.add "RNA" ("Ravnica Allegiance", 283u, "2019-01-25")
        |> Map.add "WAR" ("War of the Spark", 264u, "2019-05-03")
        |> Map.add "ELD" ("Throne of Eldraine", 269u, "2019-10-04")
        |> Map.add "THD" ("Theros Beyond Death", 254u, "2020-01-24")
        |> Map.add "IKO" ("Ikoria: Lair of Behemoths", 254u, "2020-04-17")
        |> Map.add "M21" ("Core Set 2021", 274u, "2020-07-03")
