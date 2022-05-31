using System;
using System.Collections.Generic;
using RimGoddess.Base;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using ThingDefOf = RimGoddess.Race.Definition.ThingDefOf;

namespace RimGoddess.Race;

public class PawnTransformationController : IExposable
{
    private readonly GoddessPawn m_pawn;
    private BuildingFaithPedestal m_assignedFaithPedestal;

    private bool m_stateUpdate = true;

    private bool m_transformed;

    public PawnTransformationController(GoddessPawn a_pawn)
    {
        m_pawn = a_pawn;
    }

    public bool Transformed
    {
        get => m_transformed;
        set
        {
            if (m_transformed == value)
            {
                return;
            }

            m_stateUpdate = true;
            m_transformed = value;
        }
    }

    public bool TransformCached
    {
        get
        {
            if (m_transformed)
            {
                return true;
            }

            if (!m_transformed)
            {
                return m_stateUpdate;
            }

            return false;
        }
    }

    public BuildingFaithPedestal AssignedFaithPedestal
    {
        get => m_assignedFaithPedestal;
        set => m_assignedFaithPedestal = value;
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref m_transformed, "transformed");
        Scribe_References.Look(ref m_assignedFaithPedestal, "assignedFaithPedestal");
        if (Scribe.mode == LoadSaveMode.PostLoadInit && m_assignedFaithPedestal != null)
        {
            m_assignedFaithPedestal.TryAssignPawn(m_pawn);
        }
    }

    private void ClearPawnData()
    {
        if (m_pawn.IsWorldPawn())
        {
            return;
        }

        RegionListersUpdater.DeregisterInRegions(m_pawn, m_pawn.Map);
        m_pawn.Map.listerThings.Remove(m_pawn);
        if (m_pawn.Spawned)
        {
            m_pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(m_pawn);
        }

        m_pawn.mindState.priorityWork.ClearPrioritizedWorkAndJobQueue();
        m_pawn.jobs.ClearQueuedJobs();
        if (m_pawn.jobs.curJob != null && m_pawn.jobs.IsCurrentJobPlayerInterruptible())
        {
            m_pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
        }
    }

    private void AddPawnData()
    {
        if (m_pawn.IsWorldPawn())
        {
            return;
        }

        RegionListersUpdater.RegisterInRegions(m_pawn, m_pawn.Map);
        m_pawn.Map.listerThings.Add(m_pawn);
    }

    private void CallTransform(ThingWithComps a_thing)
    {
        if (a_thing is not ITransformable transformable)
        {
            return;
        }

        transformable.Notify_Transformed(m_pawn, m_transformed, m_pawn.TransformedColor);
        try
        {
            a_thing.DrawColor = m_pawn.HighlightColor;
        }
        catch (Exception)
        {
            // ignored
        }

        a_thing.Notify_ColorChanged();
    }

    public void TransformationControllerTick()
    {
        var needFaith = m_pawn.needs.TryGetNeed<NeedFaith>();
        if (m_transformed && m_pawn.IsCaravanMember())
        {
            Transformed = false;
        }

        if (m_stateUpdate)
        {
            ClearPawnData();
            m_pawn.def = m_transformed ? ThingDefOf.RTN_Thing_GoddessTransformed : ThingDefOf.RTN_Thing_Goddess;
            m_pawn.Notify_Transformed();
            AddPawnData();
            m_stateUpdate = false;
            foreach (var item in m_pawn.equipment.AllEquipmentListForReading)
            {
                CallTransform(item);
            }

            foreach (var item2 in m_pawn.apparel.WornApparel)
            {
                CallTransform(item2);
            }

            LongEventHandler.ExecuteWhenFinished(delegate
            {
                m_pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
                foreach (var item3 in m_pawn.apparel.WornApparel)
                {
                    if (item3 is ITransformable transformable)
                    {
                        transformable.Notify_ShaderRefresh(m_pawn);
                    }
                }

                PortraitsCache.SetDirty(m_pawn);
            });
        }

        if (m_transformed && (needFaith.CurLevelPercentage <= 0.05 || m_pawn.Downed || m_pawn.Dead))
        {
            Transformed = false;
        }
    }

    public IEnumerable<Gizmo> GetGizmos()
    {
        var faith = m_pawn.needs.TryGetNeed<NeedFaith>();
        var command_Toggle = new Command_Toggle
        {
            isActive = () => m_transformed,
            toggleAction = delegate { Transformed = !Transformed; },
            hotKey = KeyBindingDefOf.Command_TogglePower,
            defaultDesc = "RTN_Translation_CommandTransformDesc".Translate(),
            icon = TexCommand.DesirePower,
            turnOnSound = SoundDefOf.DraftOn,
            turnOffSound = SoundDefOf.DraftOff
        };
        if (!m_transformed)
        {
            command_Toggle.defaultLabel = "RTN_Translation_CommandTransformLabel".Translate();
        }

        if (m_pawn.Downed)
        {
            command_Toggle.Disable("IsIncapped".Translate(m_pawn.LabelShort, m_pawn));
        }
        else if (faith.CurLevelPercentage <= 0.05f)
        {
            command_Toggle.Disable($"{"RTN_Translation_LowFaith".Translate()}: {m_pawn.LabelShort}");
        }

        yield return command_Toggle;
        if (m_transformed)
        {
            yield return new FaithGizmo(faith);
        }
    }
}