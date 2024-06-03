using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace NoAutoTreeCut;

[HarmonyPatch(typeof(RoofUtility), nameof(RoofUtility.HandleBlockingThingJob))]
static class Harmony_RoofUtility_HandleBlockingThingJob
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codes = new List<CodeInstruction>(instructions);

        var willingToCutIndex = codes.FindIndex(codes => codes.Calls(Harmony_Helpers.m_PawnWillingToCutPlant_Job));

        var willingToCutStartLabel = generator.DefineLabel();
        var willingToCutStartIndex = willingToCutIndex - 2;
        codes[willingToCutStartIndex].labels.Add(willingToCutStartLabel);

        codes.InsertRange(willingToCutStartIndex, [
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, Harmony_Helpers.m_CanAndShouldAddExtractTreeDesignation),
            new CodeInstruction(OpCodes.Brfalse_S, willingToCutStartLabel),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Call, Harmony_Helpers.m_TryGetJob),
            new CodeInstruction(OpCodes.Ret),
        ]);

        return codes.AsEnumerable();
    }
}
