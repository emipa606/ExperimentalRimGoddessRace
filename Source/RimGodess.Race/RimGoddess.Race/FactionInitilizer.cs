using System.Collections.Generic;
using RimGoddess.Race.Definition;
using Verse;

namespace RimGoddess.Race;

[StaticConstructorOnStartup]
public static class FactionInitilizer
{
    static FactionInitilizer()
    {
        var named = DefDatabase<ThingDef>.GetNamedSilentFail("RTN_Thing_SentinelArmourTethered");
        if (named == null)
        {
            return;
        }

        var rTN_PawnKind_HighAngelicGuard = PawnKindDefOf.RTN_PawnKind_HighAngelicGuard;
        if (rTN_PawnKind_HighAngelicGuard.apparelRequired == null)
        {
            rTN_PawnKind_HighAngelicGuard.apparelRequired = new List<ThingDef>();
        }

        rTN_PawnKind_HighAngelicGuard.apparelRequired.Clear();
        rTN_PawnKind_HighAngelicGuard.apparelRequired.Add(named);
    }
}