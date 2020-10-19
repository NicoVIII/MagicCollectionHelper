namespace MagicCollectionHelper

open System

open MagicCollectionHelper.Types

module CardData =
    let private add key = key |> Map.add

    // Data mostly from https://mtg.gamepedia.com/Set#List_of_Magic_expansions_and_sets
    // TODO: Dynamically fetch those data from an API
    let private setData =
        Map.empty
        |> add "INV" ("Invasion", 350u, "2000-10-02")
        |> add "ODY" ("Odyssey", 350u, "2001-10-01")
        |> add "RAV" ("Ravnica: City of Guilds", 306u, "2005-10-07")
        |> add "TSP" ("Timespiral", 422u, "2006-10-06")
        |> add "ALA" ("Shards of Alara", 249u, "2008-10-03")
        |> add "ARB" ("Alara Reborn", 145u, "2009-04-30")
        |> add "M10" ("Magic 2010", 249u, "2009-07-17")
        |> add "M11" ("Magic 2011", 249u, "2010-07-16")
        |> add "SOM" ("Scars of Mirrodin", 249u, "2010-10-01")
        |> add "M12" ("Magic 2012", 249u, "2011-07-15")
        |> add "ISD" ("Innistrad", 264u, "2011-09-30")
        |> add "AVR" ("Avacyn Restored", 244u, "2012-05-04")
        |> add "M13" ("Magic 2013", 249u, "2012-07-13")
        |> add "RTR" ("Return to Ravnica", 274u, "2012-10-05")
        |> add "M14" ("Magic 2014", 249u, "2013-07-19")
        |> add "THS" ("Theros", 249u, "2013-09-27")
        |> add "JOU" ("Journey into Nyx", 165u, "2014-05-02")
        |> add "M15" ("Magic 2015", 269u, "2014-07-18")
        |> add "KTK" ("Khans of Tarkir", 269u, "2014-09-26")
        |> add "BFZ" ("Battle for Zendikar", 274u, "2015-10-02")
        |> add "OGW" ("Oath of the Gatewatch", 184u, "2016-01-22")
        |> add "SOI" ("Shadows over Innistrad", 297u, "2016-04-08")
        |> add "EMA" ("Eternal Masters", 249u, "2016-06-10")
        |> add "EMN" ("Eldritch Moon", 205u, "2016-07-22")
        |> add "CN2" ("Conspiracy: Take the Crown", 221u, "2016-08-26")
        |> add "KLD" ("Kaladesh", 264u, "2016-09-30")
        |> add "AER" ("Aether Revolt", 184u, "2017-01-20")
        |> add "MM3" ("Modern Masters 2017", 249u, "2017-03-17")
        |> add "AKH" ("Amonkhet", 269u, "2017-04-28")
        |> add "HOU" ("Hour of Devastation", 199u, "2017-07-14")
        |> add "XLN" ("Ixalan", 299u, "2017-09-29")
        |> add "RIX" ("Rivals of Ixalan", 196u, "2018-01-19")
        |> add "DOM" ("Dominaria", 269u, "2018-04-27")
        |> add "M19" ("Core Set 2019", 280u, "2018-07-13")
        |> add "GRN" ("Guilds of Ravnica", 259u, "2018-10-05")
        |> add "RNA" ("Ravnica Allegiance", 259u, "2019-01-25")
        |> add "WAR" ("War of the Spark", 264u, "2019-05-03")
        |> add "M20" ("Core Set 2020", 345u, "2019-07-12")
        |> add "ELD" ("Throne of Eldraine", 269u, "2019-10-04")
        |> add "THB" ("Theros Beyond Death", 254u, "2020-01-24")
        |> add "IKO" ("Ikoria: Lair of Behemoths", 254u, "2020-04-17")
        |> add "M21" ("Core Set 2021", 274u, "2020-07-03")
        |> add "2XM" ("Double Masters", 332u, "2020-08-07")

    let tryFindByCardSet (CardSet key) = Map.tryFind key setData
    let tryFindByTokenSet (TokenSet key) = Map.tryFind key setData

    let tryFindBySet set =
        match set with
        | SetOfCards cardSet -> tryFindByCardSet cardSet
        | SetOfToken tokenSet -> tryFindByTokenSet tokenSet
