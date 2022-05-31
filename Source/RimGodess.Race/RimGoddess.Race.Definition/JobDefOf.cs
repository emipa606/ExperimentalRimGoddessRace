using RimWorld;
using Verse;

namespace RimGoddess.Race.Definition;

[DefOf]
public static class JobDefOf
{
    public static JobDef RTN_Job_TetherEquipment;

    static JobDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
    }
}