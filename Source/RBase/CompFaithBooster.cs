using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimGoddess.Base;

public class CompFaithBooster : ThingComp
{
    public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn a_selPawn)
    {
        if (a_selPawn is IGPawn && a_selPawn.CanReserveAndReach(parent, PathEndMode.ClosestTouch, Danger.Deadly))
        {
            yield return new FloatMenuOption(
                $"{"RTN_Translation_Absorb".Translate()} {parent.LabelShort}", delegate
                {
                    var job = new Job(JobDefOf.RTN_Job_AbsorbFaithBooster, parent);
                    a_selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                });
        }
    }
}