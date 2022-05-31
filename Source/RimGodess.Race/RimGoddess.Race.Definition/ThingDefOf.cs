using RimWorld;
using Verse;

namespace RimGoddess.Race.Definition;

[DefOf]
public static class ThingDefOf
{
    public static ThingDef RTN_Thing_Goddess;

    public static ThingDef RTN_Thing_GoddessTransformed;

    public static ThingDef RTN_Thing_FaithPedestal;

    public static ThingDef RTN_Thing_FaithCrystal;

    static ThingDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
    }
}