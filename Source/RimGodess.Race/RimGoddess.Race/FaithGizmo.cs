using UnityEngine;
using Verse;

namespace RimGoddess.Race;

[StaticConstructorOnStartup]
public class FaithGizmo : Gizmo
{
    private static readonly Texture2D FullBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.55f, 0.84f));

    private static readonly Texture2D EmptyBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));

    private readonly NeedFaith m_faithNeed;

    public FaithGizmo(NeedFaith a_faith)
    {
        m_faithNeed = a_faith;
        order = -200f;
    }

    public override float GetWidth(float a_maxWidth)
    {
        return 170f;
    }

    public override GizmoResult GizmoOnGUI(Vector2 a_topLeft, float a_maxWidth, GizmoRenderParms parms)
    {
        var rect = new Rect(a_topLeft.x, a_topLeft.y, GetWidth(a_maxWidth), 75f);
        Widgets.DrawWindowBackground(rect);
        Text.Font = GameFont.Tiny;
        var rect2 = rect.ContractedBy(6f);
        var rect3 = rect2;
        rect3.height = Text.LineHeight;
        Widgets.Label(rect3, "RTN_Translation_Faith".Translate());
        var rect4 = rect2;
        rect4.yMin = rect2.y + (rect2.height / 2f) + 4f;
        Widgets.FillableBar(rect4, m_faithNeed.CurLevelPercentage, FullBarTex, EmptyBarTex, false);
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect4, $"{m_faithNeed.CurLevel:F0} / {m_faithNeed.MaxLevel:F0}");
        Text.Anchor = TextAnchor.UpperLeft;
        return new GizmoResult(GizmoState.Clear);
    }
}