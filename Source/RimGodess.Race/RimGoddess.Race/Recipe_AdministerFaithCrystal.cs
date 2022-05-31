using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimGoddess.Race;

public class Recipe_AdministerFaithCrystal : Recipe_Surgery
{
    public override void ApplyOnPawn(Pawn a_pawn, BodyPartRecord a_part, Pawn a_billDoer, List<Thing> a_ingredients,
        Bill a_bill)
    {
        if (a_pawn is GoddessPawn goddessPawn)
        {
            goddessPawn.AddReserveFaith(1000f);
        }
    }
}