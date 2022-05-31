using System.Collections.Generic;
using RimGoddess.Base;
using Verse.AI;

namespace RimGoddess.Race;

public class JobDriver_TetherEquipment : JobDriver
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
            .FailOnDestroyedNullOrForbidden(TargetIndex.A);
        yield return new Toil
        {
            initAction = delegate
            {
                var buildingFaithPedestal = TargetA.Thing as BuildingFaithPedestal;
                foreach (var item in pawn.apparel.WornApparel)
                {
                    if (item is ITetherEquipment { Tethered: false } tetherEquipment)
                    {
                        buildingFaithPedestal?.AddTetherItem(tetherEquipment);
                    }
                }
            }
        };
    }
}