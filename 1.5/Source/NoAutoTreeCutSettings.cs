using Verse;

namespace NoAutoTreeCut;

public class NoAutoTreeCutSettings : ModSettings
{
    public static bool extractTrees = true;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref extractTrees, "extractTrees", true);
    }
}
