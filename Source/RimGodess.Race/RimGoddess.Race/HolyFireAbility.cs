using RimGoddess.Base;
using RimWorld;
using Verse;

namespace RimGoddess.Race;

public class HolyFireAbility : GoddessAbility
{
    public HolyFireAbility(Pawn a_pawn)
        : base(a_pawn)
    {
    }

    public HolyFireAbility(Pawn a_pawn, GoddessAbilityDef a_def)
        : base(a_pawn, a_def)
    {
    }

    public override bool Activate(LocalTargetInfo a_currentTarget, LocalTargetInfo a_currentDestination)
    {
        if (a_currentTarget.Pawn == null || !Triggered())
        {
            return false;
        }

        a_currentTarget.Pawn.TryAttachFire(1f);
        Find.BattleLog.Add(new BattleLogEntry_GoddessAbilityUsed(pawn, a_currentTarget.Thing, def,
            RulePackDefOf.Event_AbilityUsed));
        return true;
    }
}