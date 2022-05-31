using RimWorld;

namespace RimGoddess.Race.Definition;

[DefOf]
public static class GoddessAbilityDefOf
{
    public static GoddessAbilityDef RTN_GoddessAbility_HolyFire;

    static GoddessAbilityDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(GoddessAbilityDefOf));
    }
}