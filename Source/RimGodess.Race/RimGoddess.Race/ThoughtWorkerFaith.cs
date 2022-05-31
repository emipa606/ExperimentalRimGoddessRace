using RimWorld;
using Verse;

namespace RimGoddess.Race;

public class ThoughtWorkerFaith : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn a_pawn)
    {
        var needFaith = a_pawn.needs.TryGetNeed<NeedFaith>();
        if (needFaith == null)
        {
            return ThoughtState.Inactive;
        }

        switch (needFaith.NeedCatagory)
        {
            case e_NeedShareCatagory.Low:
                return ThoughtState.ActiveAtStage(0);
            case e_NeedShareCatagory.Depleted:
                return ThoughtState.ActiveAtStage(1);
        }

        return ThoughtState.Inactive;
    }
}