using Verse;

namespace NoAutoTreeCut;

[StaticConstructorOnStartup]
public static class Patcher
{
    static Patcher()
    {
        new HarmonyLib.Harmony("eth0net.NoAutoTreeCut").PatchAll();
    }
}
