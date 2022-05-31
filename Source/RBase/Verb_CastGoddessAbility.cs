using RimWorld;
using UnityEngine;
using Verse;

namespace RimGoddess.Base;

public class Verb_CastGoddessAbility : Verb
{
    public GoddessAbility ability;

    public override ITargetingSource DestinationSelector => null;

    public static Color RadiusHighlightColor => new Color(0.3f, 0.8f, 1f);

    public override string ReportLabel => ability.def.Label;

    public override bool MultiSelect => true;

    protected override bool TryCastShot()
    {
        return ability.Activate(currentTarget, currentDestination);
    }

    public override void OrderForceTarget(LocalTargetInfo a_target)
    {
        ability.QueueCastingJob(a_target, null);
    }

    public void DrawRadius()
    {
        GenDraw.DrawRadiusRing(ability.pawn.Position, verbProps.range);
    }
}