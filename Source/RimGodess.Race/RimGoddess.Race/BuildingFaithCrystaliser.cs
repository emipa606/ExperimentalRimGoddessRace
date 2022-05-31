using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using ThingDefOf = RimGoddess.Race.Definition.ThingDefOf;

namespace RimGoddess.Race;

public class BuildingFaithCrystaliser : Building
{
    private BuildingFaithPedestal m_faithPedestal;

    private float m_faithPull = 1f;

    private CompFlickable m_flick;

    private float m_progress;

    public BuildingFaithPedestal FaithPedestal
    {
        get => m_faithPedestal;
        set => m_faithPedestal = value;
    }

    public override void SpawnSetup(Map a_map, bool a_respawningAfterLoad)
    {
        base.SpawnSetup(a_map, a_respawningAfterLoad);
        m_flick = GetComp<CompFlickable>();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref m_faithPedestal, "faithPedestal");
        Scribe_Values.Look(ref m_progress, "progress");
    }

    public override void Tick()
    {
        if (m_faithPedestal == null)
        {
            var position = Position;
            var c = new IntVec3(position.x, position.y, position.z + 3);
            m_faithPedestal = Map.thingGrid.ThingAt<BuildingFaithPedestal>(c);
            _ = m_faithPedestal;
        }

        if (m_flick.SwitchIsOn)
        {
            m_faithPedestal.FaithDrain += m_faithPull;
            var val = m_faithPull / 60f;
            var num = Math.Min(m_faithPedestal.Faith, val);
            if (num != 0f)
            {
                m_faithPedestal.Faith -= num;
                m_progress += num / 10000f;
            }

            if (m_progress >= 1f)
            {
                m_progress -= 1f;
                var thing = ThingMaker.MakeThing(ThingDefOf.RTN_Thing_FaithCrystal);
                thing.stackCount = 1;
                GenPlace.TryPlaceThing(thing, Position, Map, ThingPlaceMode.Near);
            }
        }

        base.Tick();
    }

    public override string GetInspectString()
    {
        var text = string.Empty;
        if (m_flick.SwitchIsOn)
        {
            text += $"{"RTN_Translation_FaithDraw".Translate()}: {m_faithPull.ToString()}\n";
        }

        text += $"{"RTN_Translation_CrystalisationProgress".Translate()}: {(int)(m_progress * 100f)}%\n";
        return text.TrimEndNewlines();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (m_faithPull > 1f)
        {
            var command_Action = new Command_Action
            {
                defaultLabel = "RTN_Translation_DecreaseFaithDraw".Translate(),
                icon = TexUI.ArrowTexLeft,
                action = delegate { m_faithPull -= 1f; }
            };
            yield return command_Action;
        }

        if (!(m_faithPull < 10f))
        {
            yield break;
        }

        var command_Action2 = new Command_Action
        {
            defaultLabel = "RTN_Translation_IncreaseFaithDraw".Translate(),
            icon = TexUI.ArrowTexRight,
            action = delegate { m_faithPull += 1f; }
        };
        yield return command_Action2;
    }
}