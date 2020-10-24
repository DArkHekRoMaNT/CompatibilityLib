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
                foreach (var asset in api.Assets.GetMany("compatibility/" + mod.Info.ModID))
                {
                    // Remove "compatibility/<modid>/"
                    string[] path = asset.Location.Path.Split(new[] { '/', '\\' });
                    string newpath = path[2];
                    for (int i = 3; i < path.Length; i++)
                    {
                        newpath += "/" + path[i];
                    }

                    asset.Location.Path = newpath;
                }
            }
        }
    }
}