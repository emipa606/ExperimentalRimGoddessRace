using RimWorld;
using Verse;

namespace RimGoddess.Race;

public class BlueprintFaithPedestal : Blueprint_Build
{
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        BuildingFaithPedestal.AddFaithPedestal(Faction);
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        base.Destroy(mode);
        BuildingFaithPedestal.RemovePedestal(Faction);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        base.DeSpawn(mode);
        BuildingFaithPedestal.RemovePedestal(Faction);
    }
}