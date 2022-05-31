using RimWorld;
using Verse;

namespace RimGoddess.Race;

public class BuildingEnergyConverter : Building
{
    private const float FAITH_GAIN = 10f;

    private BuildingFaithPedestal m_faithPedestal;

    private CompPowerTrader m_powerTrader;

    public BuildingFaithPedestal FaithPedestal
    {
        get => m_faithPedestal;
        set => m_faithPedestal = value;
    }

    public bool Enabled => m_powerTrader.PowerOn;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref m_faithPedestal, "faithPedestal");
    }

    public override void SpawnSetup(Map a_map, bool a_respawningAfterLoad)
    {
        base.SpawnSetup(a_map, a_respawningAfterLoad);
        m_powerTrader = GetComp<CompPowerTrader>();
    }

    public override string GetInspectString()
    {
        return (string.Empty +
                $"{"RTN_Translation_GainRate".Translate()}: {10f}{"RTN_Translation_FaithPerSecond".Translate()} \n")
            .TrimEndNewlines();
    }

    public override void Tick()
    {
        if (m_faithPedestal == null)
        {
            var position = Position;
            var c = new IntVec3(position.x, position.y, position.z - 2);
            m_faithPedestal = Map.thingGrid.ThingAt<BuildingFaithPedestal>(c);
            if (m_faithPedestal != null)
            {
                m_faithPedestal.EnergyConverter = this;
            }
        }

        if (m_faithPedestal != null)
        {
            m_faithPedestal.FaithGain = m_powerTrader.PowerOn ? 10f : 0f;
        }

        base.Tick();
    }
}