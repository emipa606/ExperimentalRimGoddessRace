using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RimGoddess.Race;

[StaticConstructorOnStartup]
public static class InternalBackstoryDatabase
{
    private static readonly List<BackstoryDef> GoddessBackstories;

    private static readonly List<BackstoryDef> HandmaidenBackstories;

    static InternalBackstoryDatabase()
    {
        GoddessBackstories = DefDatabase<BackstoryDef>.AllDefsListForReading
            .Where(a_def => a_def.titleShort == "Goddess").ToList();
        HandmaidenBackstories = DefDatabase<BackstoryDef>.AllDefsListForReading
            .Where(a_def => a_def.titleShort == "Handmaiden").ToList();
    }


    public static BackstoryDef RandomGoddessBackstory(BackstorySlot a_slot)
    {
        return GoddessBackstories.RandomElement();
    }

    public static BackstoryDef RandomMaidenBackstory(BackstorySlot a_slot)
    {
        return HandmaidenBackstories.RandomElement();
    }

    public static BackstoryDef GetMaidenBackstory(string identifier)
    {
        return HandmaidenBackstories.Find(a_def => a_def.defName == identifier);
    }

    public static BackstoryDef GetGoddessBackstory(string identifier)
    {
        return GoddessBackstories.Find(a_def => a_def.defName == identifier);
    }
}