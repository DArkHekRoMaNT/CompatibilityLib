# CompatibilityLib

Topic on forum: https://www.vintagestory.at/forums/topic/2863-combatibility-lib

A simple library for Vintage Story that makes it easy to add compatibility with other mods for assets. 

### Easy way
You just need to add your asset to `assets/<yourmodid>/compatibility/<othermodid>/<vanilla-path>`. They will only be loaded if `<othermodid>` is loaded.

For example:<br/>
I have a More Variants mod (modid - morevariants), I want to add a patch to support Carry Capacity (modid - carrycapacity) and add recipes for Extra Chests (modid - extrachests). As a result, the assets will look like this:

- assets
  - morevariants
    - blocktypes
    - patches
    - recipes
    - compatibility
      - carrycapacity
        - patches
          - carryable.json
      - betterchests
        - recipes
          - grid
            - copperchest.json
            - copperlabeledchest.json

P.S. If `assets/<yourmodid>/<vanilla-path>` and `assets/<yourmodid>/compatibility/<othermodid>/<vanilla-path>` exists then if `<othermodid>` is loaded, the first asset will be replaced.

### Advanced (v1.2+)
You can use dependsOn[] in json-patch for create mod-dependent patch. For example:

- `dependsOn[{"modid": "morerecipes"}]` - loaded if enabled morerecipes mod
- `dependsOn[{"modid": "morerecipes", "invert": true}]` - loaded if disabled morerecipes mod
- `dependsOn[{"modid": "morerecipes"}, {"modid": "captureanimals"}]` - loaded if enabled morerecipes AND captureanimals mods
- `dependsOn[{"modid": "morerecipes"}, {"modid": "captureanimals", "invert": true}]` - loaded if enabled morerecipes AND  disabled captureanimals

**Warning!** Unlike the easy way, if you use mod-dependent patches, it is advisable to add compatibilitylib in the mod dependencies, otherwise all patches will be loaded without the library installed.

How to add to modinfo.json:
```
{
  "modid": "bestmod",
  <...>
  "dependencies": {
    "game": "1.13.4", <-- minimal game version for your mod
    "compatibilitylib": "1.2.0" <-- minimal CL version
  }
}
```

Full patch example:
```json
[
  {
    "_comment": "If you add enabled: false to your recipe, you can simply enable it when the desired mod is loaded",
    "file": "recipes/grid/best-other-fish-recipe.json",
    "op": "replace",
    "path": "/enabled",
    "value": true,
    "dependsOn": [{ "modid": "primitivesurvival" }]
  },
  {
    "_comment": "Otherwise, just disable the recipe when the mod is not loaded",
    "file": "recipes/grid/best-fish-recipe.json",
    "op": "add",
    "path": "/enabled",
    "value": false,
    "dependsOn": [{ "modid": "primitivesurvival", "invert": true }]
  },
  {
    "_comment": "Or when two mods are loaded :P",
    "file": "recipes/grid/best-fish-recipe-with-acorns.json",
    "op": "replace",
    "path": "/enabled",
    "value": true,
    "dependsOn": [
        { "modid": "primitivesurvival" },
        { "modid": "acorns" }
     ]
  },
  { "_comment": "For simplicity, you can patch all recipes in a folder at once with *",
    "file": "recipes/grid/morerecipes-disable/*",
    "op": "add",
    "path": "/enabled",
    "value": false,
    "dependsOn": [{ "modid": "morerecipes" }]
  }
]
```

