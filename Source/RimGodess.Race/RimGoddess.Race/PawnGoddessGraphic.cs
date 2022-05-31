using RimGoddess.Race.Definition;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimGoddess.Race;

public class PawnGoddessGraphic : IExposable
{
    private readonly GoddessPawn m_pawn;
    private Graphic m_backGraphic;

    private BackGraphicDef m_backGraphicDef;

    private Graphic m_headGraphic;

    private HeadGraphicDef m_headGraphicDef;

    public PawnGoddessGraphic(GoddessPawn a_pawn)
    {
        m_pawn = a_pawn;
        m_headGraphicDef = DefDatabase<HeadGraphicDef>.GetRandom();
        if (Rand.Chance(0.2f))
        {
            m_backGraphicDef = DefDatabase<BackGraphicDef>.GetRandom();
        }
    }

    public void ExposeData()
    {
        Scribe_Defs.Look(ref m_headGraphicDef, "headGraphicDef");
        Scribe_Defs.Look(ref m_backGraphicDef, "backGraphicDef");
    }

    public void RefreshGraphics(Color a_highlightColor, Color a_color)
    {
        if (m_headGraphic == null)
        {
            m_headGraphicDef = DefDatabase<HeadGraphicDef>.GetRandom();
        }

        var graphicData = m_headGraphicDef.graphicData;
        graphicData.color = a_color;
        graphicData.colorTwo = a_highlightColor;
        graphicData.CopyFrom(graphicData);
        m_headGraphic = graphicData.Graphic;
        if (m_backGraphicDef == null)
        {
            return;
        }

        var graphicData2 = m_backGraphicDef.graphicData;
        graphicData2.color = a_color;
        graphicData2.colorTwo = a_highlightColor;
        graphicData2.CopyFrom(graphicData2);
        m_backGraphic = graphicData2.Graphic;
    }

    private bool InBed()
    {
        return m_pawn.CurrentBed() != null;
    }

    private void DrawBack(bool a_under, float a_angle, Vector3 a_drawLoc, Rot4 a_rotation)
    {
        if (a_under)
        {
            var asInt = a_rotation.AsInt;
            if ((uint)(asInt - 1) > 2u)
            {
                return;
            }

            var quaternion = Quaternion.AngleAxis(a_angle, Vector3.up);
            var vector = quaternion * m_backGraphicDef.graphicData.DrawOffsetForRot(a_rotation);
            var mat = m_backGraphic.MatAt(a_rotation);
            GenDraw.DrawMeshNowOrLater(m_backGraphic.MeshAt(a_rotation), a_drawLoc + vector, quaternion, mat,
                false);
        }
        else if (a_rotation.AsInt == 0)
        {
            var quaternion2 = Quaternion.AngleAxis(a_angle, Vector3.up);
            var vector2 = quaternion2 * m_backGraphicDef.graphicData.DrawOffsetForRot(a_rotation);
            var mat2 = m_backGraphic.MatAt(a_rotation);
            GenDraw.DrawMeshNowOrLater(m_backGraphic.MeshAt(a_rotation), a_drawLoc + vector2, quaternion2, mat2, false);
        }
    }

    public Rot4 LayingFacing()
    {
        if (m_pawn.GetPosture() == PawnPosture.LayingOnGroundFaceUp)
        {
            return Rot4.South;
        }

        return (m_pawn.thingIDNumber % 4) switch
        {
            0 => Rot4.South,
            1 => Rot4.South,
            2 => Rot4.East,
            3 => Rot4.West,
            _ => Rot4.Random
        };
    }

    public float BodyAngle()
    {
        if (m_pawn.GetPosture() == PawnPosture.Standing)
        {
            return 0f;
        }

        var building_Bed = m_pawn.CurrentBed();
        if (building_Bed != null && m_pawn.RaceProps.Humanlike)
        {
            var rotation = building_Bed.Rotation;
            rotation.AsInt += 2;
            return rotation.AsAngle;
        }

        if (m_pawn.Downed || m_pawn.Dead)
        {
            return m_pawn.Drawer.renderer.wiggler.downedAngle;
        }

        return LayingFacing().AsAngle;
    }

    public void DrawUnderAt(Vector3 a_drawLoc, bool a_flip)
    {
        if (m_backGraphicDef == null || InBed())
        {
            return;
        }

        if (m_pawn.GetPosture() == PawnPosture.Standing)
        {
            DrawBack(true, BodyAngle(), a_drawLoc, m_pawn.Rotation);
            return;
        }

        var a_drawLoc2 = a_drawLoc;
        a_drawLoc2.y = AltitudeLayer.ItemImportant.AltitudeFor();
        DrawBack(true, BodyAngle(), a_drawLoc2, LayingFacing());
    }

    public void DrawOverAt(Vector3 a_drawLoc, bool a_flip)
    {
        var rotation = m_pawn.Rotation;
        var num = BodyAngle();
        if (!m_pawn.IsMaiden)
        {
            var quaternion = Quaternion.AngleAxis(num, Vector3.up);
            var vector = quaternion * m_headGraphicDef.graphicData.DrawOffsetForRot(rotation);
            var mat = m_headGraphic.MatAt(rotation);
            GenDraw.DrawMeshNowOrLater(m_headGraphic.MeshAt(rotation), a_drawLoc + vector, quaternion, mat, false);
        }

        if (m_backGraphic == null || InBed())
        {
            return;
        }

        if (m_pawn.GetPosture() == PawnPosture.Standing)
        {
            var a_drawLoc2 = a_drawLoc;
            a_drawLoc2.y += 0.05f;
            DrawBack(false, num, a_drawLoc2, m_pawn.Rotation);
        }
        else
        {
            var a_drawLoc3 = a_drawLoc;
            a_drawLoc3.y = AltitudeLayer.ItemImportant.AltitudeFor() + 0.05f;
            DrawBack(false, num, a_drawLoc3, LayingFacing());
        }
    }
}