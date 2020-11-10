using System.Collections.Generic;
using Vintagestory.API.Common;

[assembly: ModInfo("CompatibilityLib", "compatibilitylib",
    Authors = new[] { "DArkHekRoMaNT" },
    Version = "1.0.0",
    Website = "https://github.com/darkhekromant/CompatibilityLib")]
[assembly: ModDependency("game", "1.13.0")]

namespace CompatibilityLib
{
    public class CompatibilityLib : ModSystem
    {
        public static AssetCategory compatibility = new AssetCategory("compatibility", false, EnumAppSide.Universal);
        public override double ExecuteOrder()
        {
            return 0.04; //load before json patching
        }
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            foreach (var mod in api.ModLoader.Mods)
            {
                string prefix = "compatibility/" + mod.Info.ModID;
                foreach (var asset in api.Assets.GetMany(prefix))
                {
                    AssetLocation newLoc = asset.Location;
                    newLoc.Path = asset.Location.Path.Remove(0, prefix.Length + 1); //remove "<prefix>/"

                    //Remove existing assets (if exists)
                    if (api.Assets.AllAssets.ContainsKey(newLoc)) api.Assets.AllAssets.Remove(newLoc);

                    asset.Location.Path = newLoc.Path;
                }
            }
        }
    }
}