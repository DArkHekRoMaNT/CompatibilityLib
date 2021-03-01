using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.ServerMods.NoObf;

namespace CompatibilityLib
{
    [HarmonyPatch(typeof(ModJsonPatchLoader))]
    public class ModJsonPatchLoaderPatch
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(ModJsonPatchLoader.Start))]
        public static IEnumerable<CodeInstruction> Start_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo m_ToObject = AccessTools.Method(typeof(IAsset), nameof(IAsset.ToObject), null, new Type[] { typeof(JsonPatch[]) });
            MethodInfo m_ToObject_Ext = AccessTools.Method(typeof(IAsset), nameof(IAsset.ToObject), null, new Type[] { typeof(JsonPatchExt[]) });
            MethodInfo m_skipAsset = AccessTools.Method(typeof(ModJsonPatchLoaderPatch), nameof(SkipAsset), new Type[] { typeof(Dependence[]) });

            FieldInfo f_dependsOn = AccessTools.Field(typeof(JsonPatchExt), nameof(JsonPatchExt.dependsOn));


            bool flag1 = true;
            bool flag2 = true;

            var codes = new List<CodeInstruction>(instructions);
            yield return new CodeInstruction(codes[0]);
            for (int i = 1; i < codes.Count; i++)
            {
                if (flag1 && codes[i].Calls(m_ToObject))
                {
                    yield return new CodeInstruction(OpCodes.Callvirt, m_ToObject_Ext);
                    flag1 = false;
                    continue;
                }

                if (flag2 && (codes[i].operand as LocalBuilder)?.LocalType == typeof(JsonPatch) &&
                    codes[i - 1].opcode == OpCodes.Stloc_S && codes[i].opcode == OpCodes.Ldloc_S)
                {
                    // IL_009a: ldc.i4.0
                    // IL_009b: stloc.s 10
                    // IL_009d: br IL_0150
                    // // loop start (head: IL_0150)
                    //     IL_00a2: ldloc.s 8
                    //     IL_00a4: ldloc.s 10
                    //     IL_00a6: ldelem.ref
                    //     IL_00a7: stloc.s 11

                    // --> inject here

                    //     IL_00a9: ldloc.s 11
                    //     IL_00ab: ldfld class Vintagestory.ServerMods.NoObf.PatchCondition Vintagestory.ServerMods.NoObf.JsonPatch::Condition /* 0400010C */
                    //     IL_00b0: brfalse.s IL_012f
                    //     IL_00b2: ldarg.0
                    //     IL_00b3: ldfld class [VintagestoryAPI]Vintagestory.API.Datastructures.ITreeAttribute Vintagestory.ServerMods.NoObf.ModJsonPatchLoader::worldConfig /* 0400010F */
                    //     IL_00b8: ldloc.s 11
                    //     IL_00ba: ldfld class Vintagestory.ServerMods.NoObf.PatchCondition Vintagestory.ServerMods.NoObf.JsonPatch::Condition /* 0400010C */
                    //     IL_00bf: ldfld string Vintagestory.ServerMods.NoObf.PatchCondition::When /* 04000104 */
                    //     IL_00c4: callvirt instance class [VintagestoryAPI]Vintagestory.API.Datastructures.IAttribute [VintagestoryAPI]Vintagestory.API.Datastructures.ITreeAttribute::get_Item(string) /* 0A0002D9 */
                    //     IL_00c9: stloc.s 12
                    //     IL_00cb: ldloc.s 12
                    // --> unnecessary branch (skipped)
                    //     IL_00cd: brfalse.s IL_014a

                    //     IL_00cf: ldloc.s 11
                    //     IL_00d1: ldfld class Vintagestory.ServerMods.NoObf.PatchCondition Vintagestory.ServerMods.NoObf.JsonPatch::Condition /* 0400010C */
                    //     IL_00d6: ldfld bool Vintagestory.ServerMods.NoObf.PatchCondition::useValue /* 04000106 */
                    // --> move to loop end 
                    //     IL_00db: brfalse.s IL_00f7

                    yield return new CodeInstruction(codes[i]);
                    yield return new CodeInstruction(OpCodes.Isinst, typeof(JsonPatchExt));
                    yield return new CodeInstruction(OpCodes.Ldfld, f_dependsOn);
                    yield return new CodeInstruction(OpCodes.Call, m_skipAsset);

                    bool loc_flag = false;
                    for (int j = i; j < codes.Count; j++)
                    {
                        if (codes[j].opcode == OpCodes.Brfalse_S)
                        {
                            if (loc_flag)
                            {
                                yield return new CodeInstruction(OpCodes.Brtrue_S, codes[j].operand);
                                break;
                            }
                            loc_flag = true; // skip one
                        }
                    }

                    flag2 = false;
                }

                yield return codes[i];
            }
        }

        public static bool SkipAsset(Dependence[] dependsOn)
        {
            if (dependsOn == null) return false;

            bool flag = true;

            foreach (var dependence in dependsOn)
            {
                bool loaded = Core.LoadedModIds.Contains(dependence.modid);
                flag = flag && (loaded ^ dependence.invert);
            }

            return !flag;
        }
    }

    public class JsonPatchExt : JsonPatch
    {
        public Dependence[] dependsOn;
    }

    public class Dependence
    {
        public string modid;
        public bool invert = false;
    }
}