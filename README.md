# MagicCollectionHelper
[![Gitpod ready-to-code](https://img.shields.io/badge/Gitpod-ready--to--code-blue?logo=gitpod)](https://gitpod.io/#https://github.com/NicoVIII/MagicCollectionHelper)

This is (for now) a small tool which helps analysing MTG collections exported as CSV
from the brilliant website <https://deckstats.net>.

To run it you can download it from the release page (there is always a release with the latest code)
and execute it.
Your exported data from deckstats has to be placed inside of the same directory as the executable is.
It is typically called something like `collection_000000_0000_0000000000.csv`.

When starting the program and importing your collection, the program will take the newest csv inside
of the folder of the executable.

Imported collections are not saved for now. You have to import again after every start.

You can adjust your preferences in the preferences tab of the application.

For now this project is very experimental. But maybe if I have the time and motivation I will
further work on this and make it easier to use and more powerful.

### Inventory

Now you can automatically sort your cards into locations by certain rules. This helps organising your
collection and check, if everything is where it should be.
You need to include set (often set number) and language so that the cards can be used in the inventory.

To define your location you have to use a json for now, but you will be able to define them inside of
the application soon.

The json is located at ~/.local/share/magic-collection-helper/locations.json
(Windows: %appdata%/magic-collection-helper/save/locations.json) and looks like this for me atm:

```json
{
  "data": [
    {
      "name": "Collection GRN",
      "rules": {
        "inSet": [
          "GRN",
          "TGRN"
        ],
        "inLanguage": "en",
        "isFoil": false,
        "isToken": null,
        "typeContains": null,
        "typeNotContains": null,
        "limit": null,
        "limitExact": 1,
        "rarity": null,
        "colorIdentity": null
      },
      "sortBy": [
        "BySet",
        "ByCollectorNumber"
      ]
    },
    [..]
    {
      "name": "Collection KHM",
      "rules": {
        "inSet": [
          "KHM",
          "TKHM"
        ],
        "inLanguage": "en",
        "isFoil": false,
        "isToken": null,
        "typeContains": null,
        "typeNotContains": null,
        "limit": null,
        "limitExact": 1,
        "rarity": null,
        "colorIdentity": null
      },
      "sortBy": [
        "BySet",
        "ByCollectorNumber"
      ]
    },
    {
      "name": "Foil basic lands",
      "rules": {
        "inSet": null,
        "inLanguage": null,
        "isFoil": true,
        "isToken": null,
        "typeContains": [
          "Basic Land"
        ],
        "typeNotContains": null,
        "limit": null,
        "limitExact": 1,
        "rarity": null,
        "colorIdentity": null
      },
      "sortBy": [
        "ByColorIdentity",
        "BySet",
        "ByCollectorNumber"
      ]
    },
    {
      "name": "Lookup 1 (Lands)",
      "rules": {
        "inSet": null,
        "inLanguage": null,
        "isFoil": null,
        "isToken": null,
        "typeContains": [
          "Land"
        ],
        "typeNotContains": [
          "Basic Land"
        ],
        "limit": 1,
        "limitExact": null,
        "rarity": [
          "Rare",
          "Mythic",
          "Special",
          "Bonus"
        ],
        "colorIdentity": null
      },
      "sortBy": [
        "ByColorIdentity",
        "BySet"
      ]
    },
    {
      "name": "Lookup 1 (Colorless)",
      "rules": {
        "inSet": null,
        "inLanguage": null,
        "isFoil": null,
        "isToken": false,
        "typeContains": null,
        "typeNotContains": [
          "Land"
        ],
        "limit": 1,
        "limitExact": null,
        "rarity": [
          "Rare",
          "Mythic",
          "Special",
          "Bonus"
        ],
        "colorIdentity": [
          []
        ]
      },
      "sortBy": [
        {
          "ByTypeContains": [
            "Land",
            "Creature",
            "Sorcery",
            "Instant",
            "Enchantment",
            "Artifact",
            "Planeswalker"
          ]
        },
        "ByCmc",
        "BySet"
      ]
    },
    {
      "name": "Lookup 2 (Lands)",
      "rules": {
        "inSet": null,
        "inLanguage": null,
        "isFoil": null,
        "isToken": null,
        "typeContains": [
          "Land"
        ],
        "typeNotContains": [
          "Basic Land"
        ],
        "limit": 1,
        "limitExact": null,
        "rarity": [
          "Common",
          "Uncommon"
        ],
        "colorIdentity": null
      },
      "sortBy": [
        "ByColorIdentity",
        "ByName"
      ]
    },
    {
      "name": "Lookup 2 (Colorless)",
      "rules": {
        "inSet": null,
        "inLanguage": null,
        "isFoil": null,
        "isToken": false,
        "typeContains": null,
        "typeNotContains": [
          "Land"
        ],
        "limit": 1,
        "limitExact": null,
        "rarity": [
          "Common",
          "Uncommon"
        ],
        "colorIdentity": [
          []
        ]
      },
      "sortBy": [
        {
          "ByTypeContains": [
            "Land",
            "Creature",
            "Sorcery",
            "Instant",
            "Enchantment",
            "Artifact",
            "Planeswalker"
          ]
        },
        "ByCmc",
        "ByName"
      ]
    },
    {
      "name": "Lookup 1 (White)",
      "rules": {
        "inSet": null,
        "inLanguage": null,
        "isFoil": null,
        "isToken": false,
        "typeContains": null,
        "typeNotContains": [
          "Land"
        ],
        "limit": 1,
        "limitExact": null,
        "rarity": [
          "Uncommon",
          "Rare",
          "Mythic",
          "Special",
          "Bonus"
        ],
        "colorIdentity": [
          [
            "White"
          ]
        ]
      },
      "sortBy": [
        {
          "ByTypeContains": [
            "Land",
            "Creature",
            "Sorcery",
            "Instant",
            "Enchantment",
            "Artifact",
            "Planeswalker"
          ]
        },
        "ByCmc",
        {
          "ByRarity": [
            [
              "Common"
            ],
            [
              "Uncommon"
            ],
            [
              "Rare",
              "Mythic",
              "Special",
              "Bonus"
            ]
          ]
        },
        "BySet"
      ]
    },
    {
      "name": "Lookup 2 (White)",
      "rules": {
        "inSet": null,
        "inLanguage": null,
        "isFoil": null,
        "isToken": false,
        "typeContains": null,
        "typeNotContains": [
          "Land"
        ],
        "limit": 1,
        "limitExact": null,
        "rarity": [
          "Common"
        ],
        "colorIdentity": [
          [
            "White"
          ]
        ]
      },
      "sortBy": [
        {
          "ByTypeContains": [
            "Land",
            "Creature",
            "Sorcery",
            "Instant",
            "Enchantment",
            "Artifact",
            "Planeswalker"
          ]
        },
        "ByCmc",
        "ByName"
      ]
    },
    [..]
    {
      "name": "Lookup (Mixed 1)",
      "rules": {
        "inSet": null,
        "inLanguage": null,
        "isFoil": null,
        "isToken": null,
        "typeContains": null,
        "typeNotContains": null,
        "limit": 1,
        "limitExact": null,
        "rarity": null,
        "colorIdentity": [
          [
            "White",
            "Blue"
          ],
          [
            "White",
            "Green"
          ],
          [
            "Blue",
            "Black"
          ],
          [
            "Black",
            "Red"
          ],
          [
            "Red",
            "Green"
          ]
        ]
      },
      "sortBy": [
        {
          "ByTypeContains": [
            "Land",
            "Creature",
            "Sorcery",
            "Instant",
            "Enchantment",
            "Artifact",
            "Planeswalker"
          ]
        },
        "ByCmc"
      ]
    },
    {
      "name": "Lookup (Mixed 2)",
      "rules": {
        "inSet": null,
        "inLanguage": null,
        "isFoil": null,
        "isToken": null,
        "typeContains": null,
        "typeNotContains": null,
        "limit": 1,
        "limitExact": null,
        "rarity": null,
        "colorIdentity": null
      },
      "sortBy": [
        "ByCmc"
      ]
    }
  ],
  "version": 1
}
```

Maybe this can help creating your own locations.
