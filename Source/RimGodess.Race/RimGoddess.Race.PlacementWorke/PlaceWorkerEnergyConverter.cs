using RimGoddess.Race.Definition;
using Verse;

namespace RimGoddess.Race.PlacementWorker;

public class PlaceWorkerEnergyConverter : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef a_checkingDef, IntVec3 a_loc, Rot4 a_rot, Map a_map,
        Thing a_thingToIgnore = null, Thing a_thing = null)
    {
        var intVec = new IntVec3(a_loc.x, a_loc.y, a_loc.z - 2);
        var thing = a_map.thingGrid.ThingAt(intVec, ThingDefOf.RTN_Thing_FaithPedestal);
        if (thing == null || thing.Position != intVec)
        {
            return "RTN_Translation_PlaceFaithPedestalNorth".Translate();
        }

        return true;
    }

    public override void PostPlace(Map a_map, BuildableDef a_def, IntVec3 a_loc, Rot4 a_rot)
    {
        base.PostPlace(a_map, a_def, a_loc, a_rot);
        var buildingEnergyConverter = a_map.thingGrid.ThingAt<BuildingEnergyConverter>(a_loc);
        if (buildingEnergyConverter == null)
        {
            return;
        }

        var c = new IntVec3(a_loc.x, a_loc.y, a_loc.z - 2);
        var buildingFaithPedestal = a_map.thingGrid.ThingAt<BuildingFaithPedestal>(c);
        if (buildingFaithPedestal == null)
        {
            return;
        }

        buildingFaithPedestal.EnergyConverter = buildingEnergyConverter;
        buildingEnergyConverter.FaithPedestal = buildingFaithPedestal;
    }
}