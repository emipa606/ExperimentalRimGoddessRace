using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimGoddess.Base;

public class GoddessAbility : IVerbOwner, IExposable
{
    public IGoddessAbilityDef def;

    protected Command gizmo;

    private List<Tool> m_tools;

    private List<VerbProperties> m_verbProperties;

    private VerbTracker m_verbTracker;
    public Pawn pawn;

    public GoddessAbility(Pawn a_pawn)
    {
        pawn = a_pawn;
    }

    public GoddessAbility(Pawn a_pawn, IGoddessAbilityDef a_def)
    {
        pawn = a_pawn;
        def = a_def;
        Initialize();
    }

    public bool CanCast
    {
        get
        {
            if (pawn is IGPawn iGPawn)
            {
                return iGPawn.CanCast(def.FaithCost);
            }

            return false;
        }
    }

    public void ExposeData()
    {
        var value = def as Def;
        Scribe_Defs.Look(ref value, "def");
        def = value as IGoddessAbilityDef;
        if (def == null)
        {
            return;
        }

        Scribe_Deep.Look(ref m_verbTracker, "verbTracker", this);
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            Initialize();
        }
    }

    public VerbTracker VerbTracker
    {
        get
        {
            if (m_verbTracker == null)
            {
                m_verbTracker = new VerbTracker(this);
            }

            return m_verbTracker;
        }
    }

    public List<VerbProperties> VerbProperties
    {
        get
        {
            if (m_verbProperties != null)
            {
                return m_verbProperties;
            }

            m_verbProperties = new List<VerbProperties>();
            m_verbProperties.Insert(0, def.VerbProperties);

            return m_verbProperties;
        }
    }

    public List<Tool> Tools
    {
        get
        {
            if (m_tools == null)
            {
                m_tools = new List<Tool>();
            }

            return m_tools;
        }
    }

    public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.NativeVerb;

    public Thing ConstantCaster => pawn;

    public string UniqueVerbOwnerID()
    {
        return $"GoddessAbility_{def.Label}{pawn.ThingID}";
    }

    public bool VerbsStillUsableBy(Pawn a_p)
    {
        return true;
    }

    public void Initialize()
    {
        var verbProperties = VerbProperties;
        foreach (var properties in verbProperties)
        {
            properties.verbClass = typeof(Verb_CastGoddessAbility);
        }

        if (VerbTracker.PrimaryVerb is Verb_CastGoddessAbility verb_CastGoddessAbility)
        {
            verb_CastGoddessAbility.ability = this;
        }
    }

    public void QueueCastingJob(LocalTargetInfo a_target, LocalTargetInfo a_destination)
    {
        var job = JobMaker.MakeJob(RimWorld.JobDefOf.CastAbilityOnThing);
        job.verbToUse = m_verbTracker.PrimaryVerb;
        job.targetA = a_target;
        job.targetB = a_destination;
        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }

    public bool Triggered()
    {
        if (pawn is IGPawn iGPawn)
        {
            return iGPawn.SubtractFaith(def.FaithCost);
        }

        return false;
    }

    public virtual bool Activate(LocalTargetInfo a_currentTarget, LocalTargetInfo a_currentDestination)
    {
        return false;
    }

    public virtual void AbilityTick()
    {
        VerbTracker.VerbsTick();
        var primaryVerb = VerbTracker.PrimaryVerb;
        if (!primaryVerb.WarmingUp || CanCast)
        {
            return;
        }

        primaryVerb.WarmupStance?.Interrupt();
        primaryVerb.Reset();
    }

    public virtual Command GetGizmo()
    {
        return gizmo ?? new Command_GoddessAbility(this);
    }
}