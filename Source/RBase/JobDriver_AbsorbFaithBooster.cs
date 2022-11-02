using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimGoddess.Base;

public class JobDriver_AbsorbFaithBooster : JobDriver
{
    public override bool TryMakePreToilReservations(bool a_errorOnFailed)
    {
        return pawn.Reserve(TargetA, job, 1, -1, null, a_errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedOrNull(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A);
        yield return new Toil
        {
            initAction = delegate
            {
                var thing = job.targetA.Thing;
                Thing thing2;
                if (thing.def.stackLimit > 1 && thing.stackCount > 1)
                {
                    thing2 = thing.SplitOff(1);
                }
                else
                {
                    thing2 = thing;
                    thing.DeSpawn();
                }

                if (pawn is IGPawn iGPawn)
                {
                    iGPawn.AddReserveFaith(1000f);
                }

                thing2.def.soundInteract?.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
            }
        };
    }

    public override string GetReport()
    {
        return $"{"RTN_Translation_Absorbing".Translate()} {TargetA.Thing.Label}";
    }
}