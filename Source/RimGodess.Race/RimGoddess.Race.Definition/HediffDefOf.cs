using RimWorld;
using Verse;

namespace RimGoddess.Race.Definition;

[DefOf]
public static class HediffDefOf
{
    public static HediffDef RTN_Hediff_FaithControl;

    public static HediffDef RTN_Hediff_FaithDeprivation;

    public static HediffDef RTN_Hediff_FaithLink;

    static HediffDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));
    }
}