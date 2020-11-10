# CompatibilityLib

Topic on forum: https://www.vintagestory.at/forums/topic/2863-combatibility-lib

A simple library for Vintage Story that makes it easy to add compatibility with other mods for assets. You just need to add your asset to `assets/<yourmodid>/compatibility/<othermodid>/<vanilla-path>`. They will only be loaded if `<othermodid>` is loaded.

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
