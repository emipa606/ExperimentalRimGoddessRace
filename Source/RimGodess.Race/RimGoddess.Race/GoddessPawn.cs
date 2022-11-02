using System.Collections.Generic;
using RimGoddess.Base;
using RimGoddess.Race.Definition;
using RimWorld;
using UnityEngine;
using Verse;
using HediffDefOf = RimGoddess.Race.Definition.HediffDefOf;
using PawnRelationDefOf = RimWorld.PawnRelationDefOf;

namespace RimGoddess.Race;

public class GoddessPawn : Pawn, IGPawn
{
    private PawnDetails m_baseForm;

    private bool m_childhoodHandmaiden;

    private Pawn m_faithfulAnimal;

    private PawnGoddessGraphic m_graphicDrawing;

    private Color m_highlightColor = new Color(0f, 0f, 0f, 0f);

    private bool m_maiden;

    private bool m_recentLoad;

    private PawnTransformationController m_transformationController;

    private PawnDetails m_transformedForm;

    public bool IsMaiden
    {
        get => m_maiden;
        set => m_maiden = value;
    }

    public bool IsChildhoodMaiden
    {
        get => m_childhoodHandmaiden;
        set => m_childhoodHandmaiden = value;
    }

    public Color HighlightColor
    {
        get => m_highlightColor;
        set => m_highlightColor = value;
    }

    public Color TransformedColor => m_transformedForm.HairColor;

    public Name TransformedName => m_transformedForm.Name;

    public Name BaseName => m_baseForm.Name;

    public Pawn FaithfulAnimal
    {
        get => m_faithfulAnimal;
        set => m_faithfulAnimal = value;
    }

    public PawnTransformationController TransformationController
    {
        get => m_transformationController;
        set => m_transformationController = value;
    }

    public PawnGoddessAbilityController GoddessAbilityController { get; set; }

    public bool CanCast(float a_cost)
    {
        var needFaith = needs.TryGetNeed<NeedFaith>();
        if (needFaith != null)
        {
            return needFaith.CurLevel > a_cost;
        }

        return false;
    }

    public void AddAbility(IGoddessAbilityDef a_def)
    {
        GoddessAbilityController?.AddAbility(a_def as GoddessAbilityDef);
    }

    public void RemoveAbility(IGoddessAbilityDef a_def)
    {
        GoddessAbilityController?.RemoveAbility(a_def as GoddessAbilityDef);
    }

    public bool SubtractFaith(float a_amount)
    {
        var needFaith = needs.TryGetNeed<NeedFaith>();
        if (needFaith == null || !(needFaith.CurLevel > a_amount))
        {
            return false;
        }

        needFaith.CurLevel -= a_amount;
        return true;
    }

    public bool AddReserveFaith(float a_amount)
    {
        var needFaith = needs.TryGetNeed<NeedFaith>();
        if (needFaith == null)
        {
            return false;
        }

        needFaith.ReserveFaith += a_amount;
        return true;
    }

    public void GenerateDetails()
    {
        string text;
        if (m_baseForm == null)
        {
            text = GoddessDescriptionGenerator.GenerateName();
            m_baseForm = new PawnDetails
            {
                Name = new NameSingle(text),
                BodyType = Rand.Value < 0.5f ? BodyTypeDefOf.Female : BodyTypeDefOf.Thin,
                Hair = PawnStyleItemChooser.RandomHairFor(this),
                HairColor = PawnHairColors.RandomHairColor(this, story.SkinColor, ageTracker.AgeBiologicalYears),
                HeadGraphicPath = story.headType.GetGraphic(story.SkinColor).GraphicPath,
                PawnStyleDef = null
            };
        }
        else
        {
            text = m_baseForm.Name.ToStringShort;
        }

        var transformedName = GoddessDescriptionGenerator.GetTransformedName(text);
        if (string.IsNullOrEmpty(transformedName))
        {
            if (m_transformedForm != null)
            {
                Log.Warning("RimGoddess - Race: Triggered naming failsafe, attempting fix.");
                m_transformedForm.Name = new NameSingle(m_maiden
                    ? $"{m_transformedForm.Name.ToStringShort} Handmaiden"
                    : $"Divine {m_transformedForm.Name.ToStringShort}");
            }
            else
            {
                Log.Error(
                    "RimGoddess - Race: Invalid Name this should not be happening. Please notify mod author with details about error.");
                if (m_transformedForm != null)
                {
                    m_transformedForm.Name = new NameSingle("Error: Please Notify Author");
                }
            }
        }
        else if (m_transformedForm != null)
        {
            var isDivineOrHandmaiden = false;
            var toStringShort = m_transformedForm.Name.ToStringShort;
            if ($"{transformedName} Handmaiden" == toStringShort || $"Divine {transformedName}" == toStringShort)
            {
                isDivineOrHandmaiden = true;
            }

            if (isDivineOrHandmaiden)
            {
                m_transformedForm.Name =
                    new NameSingle(m_maiden ? $"{transformedName} Handmaiden" : $"Divine {transformedName}");
            }
            else
            {
                m_transformedForm.Name = new NameSingle(m_maiden
                    ? $"{m_transformedForm.Name.ToStringShort} Handmaiden"
                    : $"Divine {m_transformedForm.Name.ToStringShort}");
            }
        }
        else
        {
            m_transformedForm = new PawnDetails
            {
                Name = new NameSingle(m_maiden ? $"{transformedName} Handmaiden" : $"Divine {transformedName}"),
                BodyType = Rand.Value < 0.5f ? BodyTypeDefOf.Female : BodyTypeDefOf.Thin,
                Hair = PawnStyleItemChooser.RandomHairFor(this),
                HairColor = GoddessDescriptionGenerator.GetColor(text),
                HeadGraphicPath = story.headType.GetGraphic(story.SkinColor).GraphicPath
            };
            if (Rand.Chance(0.25f))
            {
                m_transformedForm.PawnStyleDef = DefDatabase<PawnStyleDef>.GetRandom();
            }
        }

        if (m_graphicDrawing == null)
        {
            m_graphicDrawing = new PawnGoddessGraphic(this);
        }

        m_graphicDrawing.RefreshGraphics(TransformedColor, m_highlightColor);
    }

    public void GenerateHighlightColor()
    {
        Color.RGBToHSV(TransformedColor, out var H, out var S, out var V);
        if (Rand.Chance(0.5f))
        {
            H = (H + Rand.Range(0.4f, 0.6f)) % 1f;
        }
        else
        {
            var num = Rand.Range(0.2f, 0.5f);
            S *= num;
            V *= num;
        }

        m_highlightColor = Color.HSVToRGB(H, S, V);
        if (m_graphicDrawing == null)
        {
            m_graphicDrawing = new PawnGoddessGraphic(this);
        }

        m_graphicDrawing.RefreshGraphics(TransformedColor, m_highlightColor);
    }

    private void SetPawnImageValues(PawnDetails a_details, PawnDetails a_other)
    {
        a_other.Name = Name;
        Name = a_details.Name;
        story.bodyType = a_details.BodyType;
        story.hairDef = a_details.Hair;
        story.HairColor = a_details.HairColor;
        Drawer.renderer.graphics.ResolveAllGraphics();
        if (a_details.PawnStyleDef != null)
        {
            var pawnStyleDef = a_details.PawnStyleDef;
            pawnStyleDef.headGraphic.color = story.SkinColor;
            pawnStyleDef.headGraphic.colorTwo = TransformedColor;
            pawnStyleDef.headGraphic.CopyFrom(pawnStyleDef.headGraphic);
            Drawer.renderer.graphics.headGraphic = pawnStyleDef.headGraphic.Graphic;
            pawnStyleDef.headGraphic.color = PawnGraphicSet.RottingColorDefault;
            pawnStyleDef.headGraphic.colorTwo = TransformedColor;
            pawnStyleDef.headGraphic.CopyFrom(pawnStyleDef.headGraphic);
            Drawer.renderer.graphics.desiccatedHeadGraphic = pawnStyleDef.headGraphic.Graphic;
            if (story.bodyType == BodyTypeDefOf.Female && pawnStyleDef.femaleBodyGraphic != null)
            {
                pawnStyleDef.femaleBodyGraphic.color = story.SkinColor;
                pawnStyleDef.femaleBodyGraphic.colorTwo = TransformedColor;
                pawnStyleDef.femaleBodyGraphic.CopyFrom(pawnStyleDef.femaleBodyGraphic);
                Drawer.renderer.graphics.nakedGraphic = pawnStyleDef.femaleBodyGraphic.Graphic;
                pawnStyleDef.femaleBodyGraphic.color = PawnGraphicSet.RottingColorDefault;
                pawnStyleDef.femaleBodyGraphic.colorTwo = TransformedColor;
                pawnStyleDef.femaleBodyGraphic.CopyFrom(pawnStyleDef.femaleBodyGraphic);
                Drawer.renderer.graphics.dessicatedGraphic = pawnStyleDef.femaleBodyGraphic.Graphic;
            }
            else if (pawnStyleDef.thinBodyGraphic != null)
            {
                pawnStyleDef.thinBodyGraphic.color = story.SkinColor;
                pawnStyleDef.thinBodyGraphic.colorTwo = TransformedColor;
                pawnStyleDef.thinBodyGraphic.CopyFrom(pawnStyleDef.thinBodyGraphic);
                Drawer.renderer.graphics.nakedGraphic = pawnStyleDef.thinBodyGraphic.Graphic;
                pawnStyleDef.thinBodyGraphic.color = PawnGraphicSet.RottingColorDefault;
                pawnStyleDef.thinBodyGraphic.colorTwo = TransformedColor;
                pawnStyleDef.thinBodyGraphic.CopyFrom(pawnStyleDef.thinBodyGraphic);
                Drawer.renderer.graphics.dessicatedGraphic = pawnStyleDef.thinBodyGraphic.Graphic;
            }
        }
        else
        {
            Drawer.renderer.graphics.headGraphic =
                story.headType.GetGraphic(story.SkinColor, false, story.SkinColorOverriden);
            Drawer.renderer.graphics.desiccatedHeadGraphic =
                story.headType.GetGraphic(PawnGraphicSet.RottingColorDefault, true, story.SkinColorOverriden);
        }
    }

    public void Notify_Transformed()
    {
        if (m_baseForm == null || m_transformedForm == null)
        {
            GenerateDetails();
        }

        if (m_highlightColor.a == 0f)
        {
            GenerateHighlightColor();
        }

        if (m_transformationController.Transformed)
        {
            SetPawnImageValues(m_transformedForm, m_baseForm);
        }
        else
        {
            SetPawnImageValues(m_baseForm, m_transformedForm);
        }
    }

    public override void Tick()
    {
        base.Tick();
        if (m_recentLoad && m_graphicDrawing != null)
        {
            m_recentLoad = false;
            m_graphicDrawing.RefreshGraphics(m_highlightColor, TransformedColor);
        }

        if (!Suspended && m_transformationController != null)
        {
            m_transformationController.TransformationControllerTick();
            if (GoddessAbilityController != null && m_transformationController.Transformed)
            {
                GoddessAbilityController.AbilityControllerTick();
            }
        }

        if (m_faithfulAnimal == null)
        {
            if (!this.IsHashIntervalTick(5000))
            {
                return;
            }

            foreach (var directRelation in relations.DirectRelations)
            {
                var otherPawn = directRelation.otherPawn;
                if (otherPawn.Dead || otherPawn.Destroyed || directRelation.def != PawnRelationDefOf.Bond ||
                    Map != otherPawn.Map || !Position.InHorDistOf(otherPawn.Position, 15f) ||
                    !GenSight.LineOfSight(Position, otherPawn.Position, Map) || !Rand.Chance(0.005f))
                {
                    continue;
                }

                relations.AddDirectRelation(Definition.PawnRelationDefOf.RTN_PawnRelation_FaithfulAnimal,
                    otherPawn);
                otherPawn.health.AddHediff(HediffDefOf.RTN_Hediff_FaithLink);
                m_faithfulAnimal = otherPawn;
                var let = LetterMaker.MakeLetter(
                    string.Format("{0}: {1}", "RTN_Translation_FaithLinkFormed".Translate(),
                        otherPawn.Name.ToStringShort),
                    "RTN_Translation_FaithLinkFormedLetter".Translate(Name.ToStringShort.Named("name"),
                        otherPawn.Name.ToStringShort.Named("animalName")), LetterDefOf.PositiveEvent,
                    new LookTargets(otherPawn, this));
                Find.LetterStack.ReceiveLetter(let);
                break;
            }

            return;
        }

        if (m_faithfulAnimal.Dead || m_faithfulAnimal.Destroyed)
        {
            m_faithfulAnimal = null;
        }
    }

    public override void DrawAt(Vector3 a_drawLoc, bool a_flip = false)
    {
        if (m_transformationController?.TransformCached == true)
        {
            m_graphicDrawing.DrawUnderAt(a_drawLoc, a_flip);
            base.DrawAt(a_drawLoc, a_flip);
            m_graphicDrawing.DrawOverAt(a_drawLoc, a_flip);
        }
        else
        {
            base.DrawAt(a_drawLoc, a_flip);
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        if (!IsColonistPlayerControlled)
        {
            yield break;
        }

        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (m_transformationController == null)
        {
            yield break;
        }

        foreach (var gizmo2 in m_transformationController.GetGizmos())
        {
            yield return gizmo2;
        }

        if (GoddessAbilityController == null || !m_transformationController.Transformed)
        {
            yield break;
        }

        foreach (var gizmo3 in GoddessAbilityController.GetGizmos())
        {
            yield return gizmo3;
        }
    }

    public override void ExposeData()
    {
        m_recentLoad = true;
        BackstoryDef backstory = null;
        if (story != null)
        {
            backstory = story.Childhood;
            story.Childhood = null;
        }

        var value = string.Empty;
        if (backstory != null)
        {
            value = backstory.identifier;
        }

        Scribe_Values.Look(ref m_maiden, "handmaiden");
        Scribe_Values.Look(ref m_childhoodHandmaiden, "childhoodHandmaiden");
        Scribe_Values.Look(ref m_highlightColor, "highlightColor", Color.black);
        Scribe_Values.Look(ref value, "backstoryChildhood");
        Scribe_Deep.Look(ref m_baseForm, "baseForm");
        Scribe_Deep.Look(ref m_transformedForm, "transformedForm");
        Scribe_Deep.Look(ref m_transformationController, "transformationController", this);
        Scribe_Deep.Look(ref m_graphicDrawing, "goddessGraphicData", this);
        Scribe_References.Look(ref m_faithfulAnimal, "linkedAnimal");
        base.ExposeData();
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (story != null)
        {
            story.Childhood = m_childhoodHandmaiden
                ? InternalBackstoryDatabase.GetMaidenBackstory(value)
                : InternalBackstoryDatabase.GetGoddessBackstory(value);
        }
    }

    private class PawnDetails : IExposable
    {
        public BodyTypeDef BodyType;

        public HairDef Hair;

        public Color HairColor;

        public string HeadGraphicPath;
        public Name Name;

        public PawnStyleDef PawnStyleDef;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref Name, "name");
            Scribe_Defs.Look(ref BodyType, "bodyType");
            Scribe_Defs.Look(ref Hair, "hair");
            Scribe_Values.Look(ref HairColor, "hairColor");
            Scribe_Values.Look(ref HeadGraphicPath, "headGraphicPath");
            Scribe_Defs.Look(ref PawnStyleDef, "pawnStyle");
        }
    }
}