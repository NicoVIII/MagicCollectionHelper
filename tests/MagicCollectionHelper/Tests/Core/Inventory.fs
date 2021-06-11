module MagicCollectionHelper.Tests.Core.Inventory

open Expecto

open MagicCollectionHelper.Core

// Global card info for the tests
let cardInfo =
    { name = "Animal Sanctuary"
      set = MagicSet.create "M21"
      collectorNumber = CollectorNumber "242"
      colors = [] |> Set.ofList
      colorIdentity = [] |> Set.ofList
      oracleId = "f3c40943-1d7c-4ea2-b34f-8df8b6775701"
      rarity = Rare
      typeLine = "Land"
      cmc = 0u }

let card =
    { foil = true
      language = Language "en"
      number = CollectorNumber "242"
      set = MagicSet.create "M21" }

let cardWithInfo = { card = card; info = cardInfo }

// Helpers so that fantomas isn't screwing up the formatting
let inventoryTests = testList "Inventory"
let fitsRulesTests = testList "fitsRules"

let fitsRules = Inventory.Rules.fitsAll [] cardWithInfo

[<Tests>]
let tests =
    inventoryTests [
        fitsRulesTests [
            // We check, if every card fits empty rules
            testProperty "noRules" (fun cardWithInfo -> Inventory.Rules.fitsAll [] cardWithInfo Rules.empty)
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

                    let result = fitsRules rules

                    $"Expected different result for card with rules: {rules}"
                    |> Expect.equal result expected)
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

                    let result = fitsRules rules

                    $"Expected different result for card with rules: {rules}"
                    |> Expect.equal result expected)
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

                    let result = fitsRules rules

                    $"Expected different result for card with rules: {rules}"
                    |> Expect.equal result expected)
            // We check type contains rule
            testCase "typeContains"
            <| (fun () ->
                let sets =
                    [ [], true
                      [ "Land" ], true
                      [ "Creature" ], false ]
                    |> List.map (fun (l, r) -> Set.ofList l, r)

                for (set, expected) in sets do
                    let rules =
                        Rules.empty |> Rules.withTypeContains set

                    let result = fitsRules rules

                    $"Expected different result for card with rules: {rules}"
                    |> Expect.equal result expected)
            // We check type not contains rule
            testCase "typeNotContains"
            <| (fun () ->
                let sets =
                    [ [], true
                      [ "Land" ], false
                      [ "Creature" ], true ]
                    |> List.map (fun (l, r) -> Set.ofList l, r)

                for (set, expected) in sets do
                    let rules =
                        Rules.empty |> Rules.withTypeNotContains set

                    let result = fitsRules rules

                    $"Expected different result for card with rules: {rules}"
                    |> Expect.equal result expected)
        ]
    ]
