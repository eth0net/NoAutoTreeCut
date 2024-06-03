using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace NoAutoTreeCut;

[HarmonyPatch(typeof(GenConstruct), nameof(GenConstruct.HandleBlockingThingJob))]
static class Harmony_GenConstruct_HandleBlockingThingJob
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codes = new List<CodeInstruction>(instructions);

        var willingToCutIndex = codes.FindIndex(codes => codes.Calls(Harmony_Helpers.m_PawnWillingToCutPlant_Job));

        var willingToCutJumpIndex = willingToCutIndex + 1;
        var jumpLabel = codes[willingToCutJumpIndex].operand;

        var beforeWillingToCutIndex = willingToCutIndex - 2;
        codes.InsertRange(beforeWillingToCutIndex, [
            new CodeInstruction(OpCodes.Ldsfld, Harmony_Helpers.f_ExtractTrees),
            new CodeInstruction(OpCodes.Brtrue_S, jumpLabel),
        ]);

        var postExtractionLabel = generator.DefineLabel();
        var searchIndex = willingToCutJumpIndex + 3; // skip the instructions inserted above
        var extractionIndex = codes.FindIndex(searchIndex, code => code.opcode == OpCodes.Ldarg_1);

        var extractTreesInstruction = new CodeInstruction(OpCodes.Ldsfld, Harmony_Helpers.f_ExtractTrees);
        extractTreesInstruction.labels.AddRange(codes[extractionIndex].ExtractLabels());
        codes[extractionIndex].labels.Add(postExtractionLabel);

        codes.InsertRange(extractionIndex, [
            extractTreesInstruction,
            new CodeInstruction(OpCodes.Brfalse_S, postExtractionLabel),
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Call, Harmony_Helpers.m_AddExtractTreeDesignation),
            new CodeInstruction(OpCodes.Brfalse_S, postExtractionLabel),
            new CodeInstruction(OpCodes.Ldnull),
            new CodeInstruction(OpCodes.Ret),
        ]);

        return codes.AsEnumerable();
    }
}
