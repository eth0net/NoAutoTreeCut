using UnityEngine;
using Verse;

namespace NoAutoTreeCut;

public class NoAutoTreeCut : Mod
{
    internal static NoAutoTreeCutSettings settings;

    public NoAutoTreeCut(ModContentPack content) : base(content)
    {
        settings = GetSettings<NoAutoTreeCutSettings>();
    }

    public override string SettingsCategory() => "No Auto Cut Trees";

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing = new Listing_Standard();

        listing.Begin(inRect);

        listing.CheckboxLabeled("Extract Trees (No Cut)", ref NoAutoTreeCutSettings.extractTrees, "Extract trees instead of cutting them when blocking construction.");

        listing.End();

        settings.Write();
    }
}
