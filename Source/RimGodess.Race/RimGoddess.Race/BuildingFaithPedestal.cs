using System;
using System.Collections.Generic;
using RimGoddess.Base;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;
using FactionDefOf = RimGoddess.Race.Definition.FactionDefOf;
using JobDefOf = RimGoddess.Race.Definition.JobDefOf;

namespace RimGoddess.Race;

public class BuildingFaithPedestal : Building
{
    public const float FAITH_DRAIN_RATE_CAP = 15f;

    private const float FAITH_CAP = 1000f;

    private const float ENERGY_CONVERTER_FAITH_CAP = 100f;

    private const float FAITH_NATURAL_GAIN = 0.05f;

    private const float GODESS_GENERATION_RATE = 5E-05f;
    private static readonly List<Faction> FaithPedestalFactions = new List<Faction>();

    private readonly List<Pawn> m_owners = new List<Pawn>();

    private BuildingEnergyConverter m_energyConverter;

    private Faction m_faction;

    private float m_faith;

    private float m_goddessGenerationProgress;

    private List<ITetherEquipment> m_tetherEquipment = new List<ITetherEquipment>();

    public BuildingEnergyConverter EnergyConverter
    {
        get => m_energyConverter;
        set => m_energyConverter = value;
    }

    public IEnumerable<Pawn> AssignedPawns => m_owners;

    public int MaxAssignedPawnsCount => 2;

    public float Faith
    {
        get => m_faith;
        set => m_faith = value;
    }

    public float FaithGain { get; set; }

    public float FaithDrain { get; set; }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref m_faith, "faith");
        Scribe_Values.Look(ref m_goddessGenerationProgress, "goddessGenerationProgress");
        Scribe_References.Look(ref m_energyConverter, "energyConverter");
        Scribe_Collections.Look(ref m_tetherEquipment, "tetherEquipment", LookMode.Reference);
    }

    public bool AssignedAnything(Pawn a_pawn)
    {
        if (a_pawn is GoddessPawn { TransformationController: { } } goddessPawn)
        {
            return goddessPawn.TransformationController.AssignedFaithPedestal != null;
        }

        return false;
    }

    public void TryAssignPawn(Pawn a_pawn)
    {
        if (a_pawn is not GoddessPawn goddessPawn || goddessPawn.TransformationController == null)
        {
            return;
        }

        goddessPawn.TransformationController.AssignedFaithPedestal?.m_owners.Remove(a_pawn);
        goddessPawn.TransformationController.AssignedFaithPedestal = this;
        m_owners.Add(a_pawn);
    }

    public void TryUnassignPawn(Pawn a_pawn)
    {
        if (m_owners.Contains(a_pawn))
        {
            m_owners.Remove(a_pawn);
        }
    }

    public override string GetInspectString()
    {
        var empty = string.Empty;
        switch (m_owners.Count)
        {
            case 0:
                empty = $"{empty}{"Owner".Translate() + ": " + "Nobody".Translate()}\n";
                break;
            case 1:
                empty = $"{empty}{"Owner".Translate() + ": " + m_owners[0].Label}\n";
                break;
            default:
            {
                empty += "Owners".Translate() + ": ";
                for (var i = 0; i < m_owners.Count; i++)
                {
                    if (i > 0)
                    {
                        empty += ", ";
                    }

                    empty += m_owners[i].LabelShort;
                }

                empty += "\n";
                break;
            }
        }

        empty += string.Format("{0}: {1}/{2}\n", "RTN_Translation_Faith".Translate(), ((int)m_faith).ToString(),
            m_energyConverter == null ? 1000f.ToString() : 100f.ToString());
        empty += string.Format("{0}: {1}/{2} {3}\n", "RTN_Translation_FaithGainLoss".Translate(),
            ((int)(FaithGain * 100f) / 100f).ToString(), ((int)(FaithDrain * 100f) / 100f).ToString(),
            "RTN_Translation_FaithPerSecond".Translate());
        if (m_goddessGenerationProgress > 0f)
        {
            empty += string.Format("{0}: {1}%\n",
                m_owners.Count == 0
                    ? "RTN_Translation_CreateGoddess".Translate()
                    : "RTN_Translation_CreateHandmaiden".Translate(),
                ((int)(m_goddessGenerationProgress * 100f)).ToString());
        }

        return empty.TrimEndNewlines();
    }

    private void GenerateGoddess()
    {
        if (GPawnGenerators.GoddessGenerator.GenerateGPawn(m_faction, Tile) is not GoddessPawn goddessPawn)
        {
            return;
        }

        m_goddessGenerationProgress = 0f;
        if (goddessPawn.TransformationController != null)
        {
            goddessPawn.TransformationController.AssignedFaithPedestal = this;
        }

        GenSpawn.Spawn(goddessPawn, InteractionCell, Map);
        goddessPawn.needs.TryGetNeed<NeedFaith>().AssignedPedestal = this;
        Faction.leader = goddessPawn;
        m_owners.Add(goddessPawn);
    }

    private void GenerateHandmaiden()
    {
        if (m_owners[0] is not IGPawn a_baseGoddess ||
            GPawnGenerators.HandmaidenGenerator.GenerateGPawn(m_faction, Tile,
                a_baseGoddess) is not GoddessPawn goddessPawn)
        {
            return;
        }

        m_goddessGenerationProgress = 0f;
        if (goddessPawn.TransformationController != null)
        {
            goddessPawn.TransformationController.AssignedFaithPedestal = this;
        }

        GenSpawn.Spawn(goddessPawn, InteractionCell, Map);
        goddessPawn.needs.TryGetNeed<NeedFaith>().AssignedPedestal = this;
        m_owners.Add(goddessPawn);
    }

    public override void SpawnSetup(Map a_map, bool a_respawningAfterLoad)
    {
        m_faction = a_map.ParentFaction;
        if (!FaithPedestalFactions.Contains(m_faction))
        {
            FaithPedestalFactions.Add(m_faction);
        }

        var faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.RTN_FactionDef_Goddess);
        if (faction != null)
        {
            if (RimWorld.FactionDefOf.Empire != null)
            {
                Faction.TryAffectGoodwillWith(Faction.OfEmpire,
                    Faction.GoodwillToMakeHostile(Faction.OfEmpire), false, false,
                    HistoryEventDefOf.UsedForbiddenThing);
            }

            Faction.TryAffectGoodwillWith(faction, Faction.GoodwillToMakeHostile(faction), false,
                false, HistoryEventDefOf.UsedForbiddenThing);


            //if (RimWorld.FactionDefOf.Empire != null)
            //{
            //    Faction.OfEmpire.SetRelationDirect(Faction, FactionRelationKind.Hostile, true,
            //        "RTN_Translation_HostilePower".Translate());
            //}

            //faction.SetRelationDirect(Faction, FactionRelationKind.Hostile, true,
            //    "RTN_Translation_HostileReligion".Translate());
        }

        base.SpawnSetup(a_map, a_respawningAfterLoad);
    }

    public override void Tick()
    {
        var notMaiden = false;
        FaithDrain = 0f;
        foreach (var owner in m_owners)
        {
            if (owner == null || owner.Dead)
            {
                continue;
            }

            var needFaith = owner.needs.TryGetNeed<NeedFaith>();
            if (needFaith == null)
            {
                continue;
            }

            FaithDrain += needFaith.DrainRate;
            if (needFaith.AssignedPedestal == null)
            {
                needFaith.AssignedPedestal = this;
            }
        }

        foreach (var item in m_tetherEquipment)
        {
            var tetherFaithDraw = item.TetherFaithDraw;
            var t = item as Thing;
            if (m_faith < 20f)
            {
                RemoveTetherItem(item);
                break;
            }

            if (!t.DestroyedOrNull())
            {
                FaithDrain += tetherFaithDraw;
                m_faith = Math.Max(0f, m_faith - (tetherFaithDraw / 60f));
                continue;
            }

            RemoveTetherItem(item);
            break;
        }

        for (var i = 0; i < m_owners.Count; i++)
        {
            var goddessPawn = m_owners[i] as GoddessPawn;
            if (goddessPawn is { Dead: false })
            {
                continue;
            }

            m_goddessGenerationProgress = 0f;
            if (goddessPawn is { IsMaiden: false })
            {
                notMaiden = true;
            }

            m_owners.RemoveAt(i);
            break;
        }

        if (notMaiden)
        {
            foreach (var pawn in m_owners)
            {
                if (pawn is not GoddessPawn goddessPawn2 || !goddessPawn2.IsMaiden || goddessPawn2.Dead)
                {
                    continue;
                }

                goddessPawn2.IsMaiden = false;
                goddessPawn2.GenerateDetails();
                var let = LetterMaker.MakeLetter(
                    string.Format("{0}: {1}", "RTN_Translation_HandmaidenPromotionTitle".Translate(),
                        goddessPawn2.Name.ToStringShort),
                    "RTN_Translation_HandmaidenPromotion".Translate(goddessPawn2.Name.ToStringShort.Named("name")),
                    LetterDefOf.PositiveEvent, goddessPawn2);
                Find.LetterStack.ReceiveLetter(let);
                Faction.leader = goddessPawn2;
                break;
            }
        }

        if (m_energyConverter != null)
        {
            if (m_energyConverter.Discarded || m_energyConverter.Destroyed)
            {
                m_energyConverter = null;
            }
        }
        else
        {
            FaithGain = 0f;
            FaithGain += 0.05f;
            foreach (var owner2 in m_owners)
            {
                var list = new List<Pawn>();
                foreach (var relatedPawn in owner2.relations.RelatedPawns)
                {
                    list.Add(relatedPawn);
                }

                List<Pawn> list2 = null;
                if (owner2.MapHeld != null)
                {
                    list2 = owner2.MapHeld.mapPawns.AllPawnsSpawned;
                }
                else if (owner2.IsCaravanMember())
                {
                    list2 = owner2.GetCaravan().PawnsListForReading;
                }

                if (list2 != null)
                {
                    foreach (var pawn in list2)
                    {
                        if (pawn.RaceProps.Humanlike && pawn != owner2 && !list.Contains(pawn) &&
                            (owner2.relations.OpinionOf(pawn) != 0 || pawn.relations.OpinionOf(owner2) != 0))
                        {
                            list.Add(owner2);
                        }
                    }
                }

                if (list2 == null)
                {
                    continue;
                }

                foreach (var item2 in list2)
                {
                    if (!item2.Dead && !item2.Downed)
                    {
                        FaithGain += item2.relations.OpinionOf(owner2) / 50f;
                    }
                }
            }
        }

        m_faith = Math.Max(Math.Min(m_energyConverter == null ? 1000f : 100f, m_faith + (FaithGain / 60f)), 0f);
        var count = m_owners.Count;
        if (count == 0 && (m_faith > 500f || m_energyConverter is { Enabled: true }))
        {
            m_goddessGenerationProgress += 8.3333333E-07f;
            if (m_goddessGenerationProgress >= 1f)
            {
                GenerateGoddess();
            }
        }
        else if (m_energyConverter == null && count == 1 && m_faith > 500f && FaithGain > 20f &&
                 m_owners[0].ageTracker.AgeBiologicalYearsFloat > 1.5f)
        {
            m_goddessGenerationProgress += 8.3333333E-07f;
            if (m_goddessGenerationProgress >= 1f)
            {
                GenerateHandmaiden();
            }
        }

        base.Tick();
    }

    public static void RemovePedestal(Faction a_faction)
    {
        if (FaithPedestalFactions.Contains(a_faction))
        {
            FaithPedestalFactions.Remove(a_faction);
        }
    }

    public override void Destroy(DestroyMode a_mode = DestroyMode.Vanish)
    {
        RemovePedestal(m_faction);
        base.Destroy(a_mode);
    }

    public override void DeSpawn(DestroyMode a_mode = DestroyMode.Vanish)
    {
        RemovePedestal(m_faction);
        base.DeSpawn(a_mode);
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (!Prefs.DevMode)
        {
            yield break;
        }

        if (m_owners.Count == 0)
        {
            var command_Action = new Command_Action
            {
                defaultLabel = "Dev: Spawn Goddess",
                action = GenerateGoddess
            };
            yield return command_Action;
        }
        else
        {
            var command_Action2 = new Command_Action
            {
                defaultLabel = "Dev: Spawn Handmaiden",
                action = GenerateHandmaiden
            };
            yield return command_Action2;
        }

        var command_Action3 = new Command_Action
        {
            defaultLabel = "Dev: Max Faith",
            action = delegate { m_faith = 1000f; }
        };
        yield return command_Action3;
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn a_selPawn)
    {
        foreach (var floatMenuOption in base.GetFloatMenuOptions(a_selPawn))
        {
            yield return floatMenuOption;
        }

        var wornApparel = a_selPawn.apparel.WornApparel;
        if (!(m_faith > 50f))
        {
            yield break;
        }

        foreach (var item in wornApparel)
        {
            if (item is not ITetherEquipment tetherEquipment || tetherEquipment.Tethered)
            {
                continue;
            }

            yield return new FloatMenuOption("RTN_Translation_TetherEquipment".Translate(), delegate
            {
                var job = new Job(JobDefOf.RTN_Job_TetherEquipment, this);
                a_selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });
            break;
        }
    }

    public void AddTetherItem(ITetherEquipment a_tetherEquipment)
    {
        m_tetherEquipment.Add(a_tetherEquipment);
        var thing = a_tetherEquipment as Thing;
        a_tetherEquipment.Notify_Tethered(((Pawn_ApparelTracker)thing?.ParentHolder)?.pawn, true);
    }

    public void RemoveTetherItem(ITetherEquipment a_tetherEquipment)
    {
        m_tetherEquipment.Remove(a_tetherEquipment);
        var thing = a_tetherEquipment as Thing;
        a_tetherEquipment.Notify_Tethered(((Pawn_ApparelTracker)thing?.ParentHolder)?.pawn, false);
    }

    public static void AddFaithPedestal(Faction a_faction)
    {
        if (!FaithPedestalFactions.Contains(a_faction))
        {
            FaithPedestalFactions.Add(a_faction);
        }
    }

    public static bool HasFaithPedestal(Faction a_faction)
    {
        return FaithPedestalFactions.Contains(a_faction);
    }
}