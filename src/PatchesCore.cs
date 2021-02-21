using Vintagestory.API.Common;
using HarmonyLib;

namespace CompatibilityLib
{
    class PatchesCore : ModSystem
    {
        public Harmony harmonyInstance;

        public override double ExecuteOrder() => 0.001;

        public override void StartPre(ICoreAPI api)
        {
            // string path = "C:/Users/dark/Desktop/harmony.log.txt";
            // if (File.Exists(path)) File.Delete(path);
            // Harmony.DEBUG = true;

            harmonyInstance = new Harmony(Mod.Info.ModID);
            harmonyInstance.PatchAll();

            api.Logger.Notification("[" + Mod.Info.ModID + "] Harmony Patched Methods: ");
            foreach (var val in harmonyInstance.GetPatchedMethods())
            {
                api.Logger.Notification("[" + Mod.Info.ModID + "]         " + val.DeclaringType + "." + val.Name);
            }
        }

        public override void Dispose()
        {
            harmonyInstance.UnpatchAll(Mod.Info.ModID);
        }
    }
}