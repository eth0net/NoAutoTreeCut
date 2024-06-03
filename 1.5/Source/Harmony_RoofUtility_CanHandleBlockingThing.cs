using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace NoAutoTreeCut;

[HarmonyPatch(typeof(RoofUtility), nameof(RoofUtility.CanHandleBlockingThing))]
static class Harmony_RoofUtility_CanHandleBlockingThing
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codes = new List<CodeInstruction>(instructions);

        var willingToCutIndex = codes.FindIndex(codes => codes.Calls(Harmony_Helpers.m_PawnWillingToCutPlant_Job));

        var willingToCutJumpIndex = willingToCutIndex + 1;
        var jumpLabel = codes[willingToCutJumpIndex].operand;

        var beforeWillingToCutIndex = willingToCutIndex - 2;
        codes.InsertRange(beforeWillingToCutIndex, [
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, Harmony_Helpers.m_CanAndShouldAddExtractTreeDesignation),
            new CodeInstruction(OpCodes.Brtrue_S, jumpLabel),
        ]);

        return codes.AsEnumerable();
    }
}
