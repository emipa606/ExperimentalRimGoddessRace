using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimGoddess.Base;

[StaticConstructorOnStartup]
internal class Command_GoddessAbility : Command
{
    public new static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/AbilityButBG");
    protected GoddessAbility m_goddessAbility;

    public Command_GoddessAbility(GoddessAbility a_goddessAbility)
    {
        var def = a_goddessAbility.def;
        m_goddessAbility = a_goddessAbility;
        order = 6f;
        defaultLabel = def.LabelCap;
        hotKey = def.HotKey;
        icon = def.UiIcon;
    }

    public override Texture2D BGTexture => BGTex;


    public override GizmoResult GizmoOnGUI(Vector2 a_topLeft, float a_maxWidth, GizmoRenderParms parms)
    {
        defaultDesc = m_goddessAbility.def.GetTooltip();
        var result = base.GizmoOnGUI(a_topLeft, a_maxWidth, parms);
        var faithCost = m_goddessAbility.def.FaithCost;
        if (faithCost > float.Epsilon)
        {
            Text.Font = GameFont.Tiny;
            var text = faithCost.ToString();
            var x = Text.CalcSize(text).x;
            Widgets.Label(new Rect(a_topLeft.x + GetWidth(a_maxWidth) - x - 5f, a_topLeft.y + 5f, x, 18f), text);
        }

        if (result.State == GizmoState.Interacted && m_goddessAbility.CanCast)
        {
            return result;
        }

        return new GizmoResult(result.State);
    }

    public override void ProcessInput(Event a_event)
    {
        base.ProcessInput(a_event);
        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
        if (m_goddessAbility.def.RequiresTarget)
        {
            Find.Targeter.BeginTargeting(m_goddessAbility.VerbTracker.PrimaryVerb);
        }
        else
        {
            m_goddessAbility.VerbTracker.PrimaryVerb.TryStartCastOn((LocalTargetInfo)m_goddessAbility.pawn);
        }
    }

    public override void GizmoUpdateOnMouseover()
    {
        if (m_goddessAbility.VerbTracker.PrimaryVerb is Verb_CastGoddessAbility verb_CastGoddessAbility &&
            m_goddessAbility.def.RequiresTarget)
        {
            verb_CastGoddessAbility.DrawRadius();
        }
    }
}