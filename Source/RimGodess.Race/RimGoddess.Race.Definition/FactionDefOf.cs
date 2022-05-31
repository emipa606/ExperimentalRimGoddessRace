using RimWorld;

namespace RimGoddess.Race.Definition;

[DefOf]
public static class FactionDefOf
{
    public static FactionDef RTN_FactionDef_Goddess;

    static FactionDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(FactionDefOf));
    }
}