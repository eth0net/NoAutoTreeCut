using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using Verse.AI;

namespace NoAutoTreeCut;

static class Harmony_Helpers
{
    internal static readonly MethodInfo m_PawnWillingToCutPlant_Job = AccessTools.Method(typeof(PlantUtility), nameof(PlantUtility.PawnWillingToCutPlant_Job));

    internal static readonly MethodInfo m_AddExtractTreeDesignation = AccessTools.Method(typeof(Harmony_Helpers), nameof(AddExtractTreeDesignation));

    internal static readonly MethodInfo m_CanAndShouldAddExtractTreeDesignation = AccessTools.Method(typeof(Harmony_Helpers), nameof(CanAndShouldAddExtractTreeDesignation));

    internal static readonly MethodInfo m_TryGetJob = AccessTools.Method(typeof(Harmony_Helpers), nameof(TryGetJob));

    internal static readonly FieldInfo f_ExtractTrees = AccessTools.Field(typeof(NoAutoTreeCutSettings), nameof(NoAutoTreeCutSettings.extractTrees));

    internal static readonly FieldInfo f_JobDefOfCutPlant = AccessTools.Field(typeof(JobDefOf), nameof(JobDefOf.CutPlant));

    static bool CanAndShouldAddExtractTreeDesignation(Thing thing)
    {
        return NoAutoTreeCutSettings.extractTrees && Find.ReverseDesignatorDatabase.Get<Designator_ExtractTree>()?.CanDesignateThing(thing) == true;
    }

    static bool AddExtractTreeDesignation(Thing thing)
    {
        if (!CanAndShouldAddExtractTreeDesignation(thing))
            return false;
        Find.ReverseDesignatorDatabase.Get<Designator_ExtractTree>().DesignateThing(thing);
        return true;
    }

    static Job TryGetJob(Thing thing, Pawn pawn)
    {
        if (!AddExtractTreeDesignation(thing))
            return null;
        var worker = DefDatabase<WorkGiverDef>.GetNamed("ExtractTree")?.Worker as WorkGiver_ExtractTree;
        return worker?.HasJobOnThing(pawn, thing) != true ? null : worker.JobOnThing(pawn, thing);
    }
}
