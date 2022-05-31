using Verse;

namespace RimGoddess.Race.PlacementWorker;

public class PlaceWorkerFaithPedestal : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef a_checkingDef, IntVec3 a_loc, Rot4 a_rot, Map a_map,
        Thing a_thingToIgnore = null, Thing a_thing = null)
    {
        if (BuildingFaithPedestal.HasFaithPedestal(a_map.ParentFaction))
        {
            return "RTN_Translation_HasFaithPedestal".Translate();
        }

        return true;
    }

    public override void PostPlace(Map a_map, BuildableDef a_def, IntVec3 a_loc, Rot4 a_rot)
    {
        base.PostPlace(a_map, a_def, a_loc, a_rot);
        BuildingFaithPedestal.AddFaithPedestal(a_map.ParentFaction);
        Find.WindowStack.Add(new Dialog_FaithPedestal());
    }
}