using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using HediffDefOf = RimGoddess.Race.Definition.HediffDefOf;

namespace RimGoddess.Race;

public class NeedFaith : Need
{
    private const float LOW_ENERGY_THRESHOLD = 20f;

    private const float DRAIN_RATE = 5f;

    private const float TRANSFORMED_DRAIN_RATE = 20f;

    private readonly GoddessPawn m_pawn;

    private bool m_disabled;

    public NeedFaith(Pawn a_pawn)
        : base(a_pawn)
    {
        m_pawn = a_pawn as GoddessPawn;
        m_disabled = m_pawn == null;
        CurLevel = MaxLevel * 0.5f;
    }

    public BuildingFaithPedestal AssignedPedestal { get; set; }

    public float DrainRate { get; private set; }

    public float ReserveFaith { get; set; }

    public override float MaxLevel => 100f;

    public override float CurLevel
    {
        get
        {
            if (m_disabled)
            {
                return MaxLevel;
            }

            return base.CurLevel;
        }
        set => base.CurLevel = value;
    }

    public e_NeedShareCatagory NeedCatagory
    {
        get
        {
            if (CurLevel <= 0f)
            {
                return e_NeedShareCatagory.Depleted;
            }

            if (CurLevel <= 20f)
            {
                return e_NeedShareCatagory.Low;
            }

            return e_NeedShareCatagory.Normal;
        }
    }

    public override bool ShowOnNeedList => !m_disabled;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref m_disabled, "disabled");
    }

    public override void NeedInterval()
    {
        m_disabled = m_pawn == null || !m_pawn.IsColonistPlayerControlled;
        if (m_disabled)
        {
            return;
        }

        DrainRate = 0f;
        DrainRate = m_pawn != null && m_pawn.TransformationController.Transformed ? 20f : 5f;

        if (m_pawn != null)
        {
            var ageChronologicalYearsFloat = m_pawn.ageTracker.AgeChronologicalYearsFloat;
            DrainRate *= Math.Min(ageChronologicalYearsFloat * ageChronologicalYearsFloat, 1f);
        }

        if (CurLevel <= 0f)
        {
            HealthUtility.AdjustSeverity(pawn, HediffDefOf.RTN_Hediff_FaithDeprivation, 0.01f);
        }
        else
        {
            HealthUtility.AdjustSeverity(pawn, HediffDefOf.RTN_Hediff_FaithDeprivation, -0.01f);
        }

        var num = 150f * (DrainRate / 60f);
        if (ReserveFaith > 0f)
        {
            var num2 = num + (MaxLevel - CurLevel);
            CurLevel += Math.Min(ReserveFaith, num2);
            ReserveFaith -= num2;
        }
        else if (AssignedPedestal != null)
        {
            var val = Math.Min(num + (MaxLevel - CurLevel), 37.5f);
            var num3 = Math.Min(AssignedPedestal.Faith, val);
            AssignedPedestal.Faith -= num3;
            CurLevel = Math.Max(CurLevel + (num3 - num), 0f);
        }
        else
        {
            CurLevel = Math.Max(CurLevel - num, 0f);
        }
    }

    public override string GetTipString()
    {
        var text = $"{base.GetTipString()}\n\n";
        text += string.Format("{0}: {1}\n", "RTN_Translation_DrainRate".Translate(),
            ((int)(DrainRate * 100f) / 100f).ToString());
        if (ReserveFaith > 0f)
        {
            text += string.Format("{0}: {1}\n", "RTN_Translation_ReserveFaith".Translate(),
                ((int)ReserveFaith).ToString());
        }

        return text.TrimEndNewlines();
    }


    public override void DrawOnGUI(Rect a_rect, int a_maxThresholdMarkers = int.MaxValue, float a_customMargin = -1f,
        bool a_drawArrows = true, bool a_doTooltip = true, Rect? rectForTooltip = null)
    {
        if (threshPercents == null)
        {
            threshPercents = new List<float>();
        }

        threshPercents.Clear();
        threshPercents.Add(20f / MaxLevel);
        base.DrawOnGUI(a_rect, a_maxThresholdMarkers, a_customMargin, a_drawArrows, a_doTooltip, rectForTooltip);
    }
}