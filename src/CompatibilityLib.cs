using System.Linq;
using Vintagestory.API.Common;

namespace CompatibilityLib
{
    public class CompatibilityLib : ModSystem
    {
        public static AssetCategory compatibility = new AssetCategory("compatibility", true, EnumAppSide.Universal);

        public static string[] partiallyWorkingCategories = { "shapes", "textures" };

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

                    //Warning about partially worked categories
                    if (partiallyWorkingCategories.Contains(asset.Location.Category.Code))
                    {
                        api.World.Logger.Warning("Asset categories: {0} - may work strange. " +
                            "It is best to leave such assets out of the compatibility category!",
                            string.Join(", ", partiallyWorkingCategories)
                        );
                    }
                }
            }
        }
    }
}