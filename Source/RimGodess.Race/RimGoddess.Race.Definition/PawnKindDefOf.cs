using RimWorld;
using Verse;

namespace RimGoddess.Race.Definition;

[DefOf]
public static class PawnKindDefOf
{
    public static PawnKindDef RTN_PawnKind_Goddess;

    public static PawnKindDef RTN_PawnKind_HighAngelicGuard;

    static PawnKindDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(PawnKindDefOf));
    }
}