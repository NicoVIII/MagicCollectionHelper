module MagicCollectionHelper.Tests.Core.Inventory

open Expecto

open MagicCollectionHelper.Core
open MagicCollectionHelper.Core.Types

// Global card info for the tests
let cardInfo =
    [ { name = "Animal Sanctuary"
        set = MagicSet.create "M21"
        collectorNumber = CollectorNumber 242u
        colors = [] |> Set.ofList
        colorIdentity = [] |> Set.ofList
        oracleId = "f3c40943-1d7c-4ea2-b34f-8df8b6775701"
        rarity = Rare } ]
    |> List.map (fun info -> (info.set, info.collectorNumber), info)
    |> Map.ofList

let card =
    { number = CollectorNumber 242u
      foil = true
      language = Language "en"
      set = MagicSet.create "M21" }

// Helpers so that fantomas isn't screwing up the formatting
let inventoryTests = testList "Inventory"
let fitsRulesTests = testList "fitsRules"

[<Tests>]
let tests =
    inventoryTests [
        fitsRulesTests [
            // We check, if every card fits empty rules
            testProperty "noRules" (fun card -> Inventory.fitsRules cardInfo [] card Rules.empty)
            // We check in set rule
            testCase "inSet"
            <| (fun () ->
                let m21 = MagicSet.create "M21"
                let rna = MagicSet.create "RNA"

                let sets =
                    [ [], false
                      [ m21 ], true
                      [ rna ], false
                      [ rna; m21 ], true ]
                    |> List.map (fun (l, r) -> Set.ofList l, r)

                for (set, expected) in sets do
                    let rules = Rules.empty |> Rules.withInSet set

                    let result =
                        Inventory.fitsRules cardInfo [] card rules

                    Expect.equal result expected "Expected different result for card")
            // We check coloridentity rule
            testCase "coloridentity"
            <| (fun () ->
                let sets =
                    [ [ [] ], true
                      [ [ White ] ], false
                      [ [ Blue ] ], false
                      [ [ Red; Black ] ], false ]
                    |> List.map (fun (l, r) -> Set.ofListList l, r)

                for (set, expected) in sets do
                    let rules =
                        Rules.empty |> Rules.withColorIdentity set

                    let result =
                        Inventory.fitsRules cardInfo [] card rules

                    Expect.equal result expected "Expected different result for card")
            // We check rarity rule
            testCase "rarity"
            <| (fun () ->
                let sets =
                    [ [], false
                      [ Rare ], true
                      [ Uncommon ], false
                      [ Uncommon; Rare ], true ]
                    |> List.map (fun (l, r) -> Set.ofList l, r)

                for (set, expected) in sets do
                    let rules = Rules.empty |> Rules.withRarity set

                    let result =
                        Inventory.fitsRules cardInfo [] card rules

                    Expect.equal result expected "Expected different result for card")
        ]
    ]
