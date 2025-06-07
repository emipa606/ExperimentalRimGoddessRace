using System.Linq;
using RimGoddess.Base;
using RimGoddess.Race.Definition;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using HediffDefOf = RimGoddess.Race.Definition.HediffDefOf;
using PawnKindDefOf = RimGoddess.Race.Definition.PawnKindDefOf;
using StatDefOf = RimWorld.StatDefOf;
using ThingDefOf = RimGoddess.Race.Definition.ThingDefOf;

namespace RimGoddess.Race;

public class GoddessGenerator : IGPawnGenerator
{
    public IGPawn GenerateGPawn(Faction a_faction, int a_tile, IGPawn a_baseGoddess = null)
    {
        var goddessPawn = (GoddessPawn)ThingMaker.MakeThing(ThingDefOf.RTN_Thing_Goddess);
        if (goddessPawn == null)
        {
            return null;
        }

        goddessPawn.kindDef = PawnKindDefOf.RTN_PawnKind_Goddess;
        goddessPawn.SetFactionDirect(a_faction);
        PawnComponentsUtility.CreateInitialComponents(goddessPawn);
        goddessPawn.TransformationController = new PawnTransformationController(goddessPawn);
        goddessPawn.GoddessAbilityController = new PawnGoddessAbilityController(goddessPawn);
        goddessPawn.gender = Gender.Female;
        goddessPawn.ageTracker.AgeBiologicalTicks = 0L;
        goddessPawn.needs.SetInitialLevels();
        goddessPawn.story.skinColorOverride = PawnSkinColors.GetSkinColor(a_faction.colorFromSpectrum);
        goddessPawn.story.headType = Rand.Value < 0.5f
            ? DefDatabase<HeadTypeDef>.GetNamedSilentFail("Female_AverageNormal")
            : DefDatabase<HeadTypeDef>.GetNamedSilentFail("Female_NarrowNormal");
        goddessPawn.story.HairColor = Color.red;
        goddessPawn.story.hairDef = PawnStyleItemChooser.RandomHairFor(goddessPawn);
        goddessPawn.story.bodyType = Rand.Value < 0.5f ? BodyTypeDefOf.Female : BodyTypeDefOf.Thin;
        goddessPawn.health.Reset();
        goddessPawn.health.AddHediff(HediffDefOf.RTN_Hediff_FaithControl);
        goddessPawn.needs.AddOrRemoveNeedsAsAppropriate();
        if (a_baseGoddess == null)
        {
            goddessPawn.story.Childhood = InternalBackstoryDatabase.RandomGoddessBackstory(BackstorySlot.Childhood);
            GenerateTraits(goddessPawn);
            GenerateSkills(goddessPawn);
            goddessPawn.IsMaiden = false;
            goddessPawn.IsChildhoodMaiden = false;
        }
        else
        {
            goddessPawn.story.Childhood = InternalBackstoryDatabase.RandomMaidenBackstory(BackstorySlot.Childhood);
            GenerateTraits(goddessPawn);
            GenerateSkills(goddessPawn);
            goddessPawn.IsMaiden = true;
            goddessPawn.IsChildhoodMaiden = true;
        }

        goddessPawn.workSettings.EnableAndInitialize();
        goddessPawn.AddAbility(GoddessAbilityDefOf.RTN_GoddessAbility_HolyFire);
        goddessPawn.GenerateDetails();
        goddessPawn.GenerateHighlightColor();
        goddessPawn.Name = goddessPawn.TransformedName;
        Find.Scenario.Notify_NewPawnGenerating(goddessPawn, PawnGenerationContext.All);
        Find.WorldObjects.WorldObjectAt<Settlement>(a_tile)?.previouslyGeneratedInhabitants.Add(goddessPawn);
        Find.Scenario.Notify_PawnGenerated(goddessPawn, PawnGenerationContext.All, false);
        if (ModsConfig.IdeologyActive)
        {
            goddessPawn.story.favoriteColor = DefDatabase<ColorDef>.AllDefsListForReading.RandomElement().color;
        }

        if (goddessPawn.Faction == Faction.OfPlayerSilentFail)
        {
            if (ModsConfig.IdeologyActive)
            {
                goddessPawn.ideo.SetIdeo(goddessPawn.Faction.ideos.GetRandomIdeoForNewPawn());
            }

            Find.StoryWatcher.watcherPopAdaptation.Notify_PawnEvent(goddessPawn, PopAdaptationEvent.GainedColonist);
        }
        else
        {
            if (!ModsConfig.IdeologyActive)
            {
                return goddessPawn;
            }

            if (Find.IdeoManager.IdeosListForReading.TryRandomElement(out var ideo))
            {
                goddessPawn.ideo.SetIdeo(ideo);
            }
        }

        return goddessPawn;
    }

    private static void GenerateTraits(Pawn a_pawn)
    {
        if (a_pawn.story == null)
        {
            return;
        }

        var num = Rand.RangeInclusive(2, 3);
        while (a_pawn.story.traits.allTraits.Count < num)
        {
            var newTraitDef =
                DefDatabase<TraitDef>.AllDefsListForReading.RandomElementByWeight(tr =>
                    tr.GetGenderSpecificCommonality(a_pawn.gender));
            if (a_pawn.story.traits.HasTrait(newTraitDef) ||
                a_pawn.story.traits.allTraits.Any(tr => newTraitDef.ConflictsWith(tr)) ||
                newTraitDef.requiredWorkTypes != null &&
                a_pawn.OneOfWorkTypesIsDisabled(newTraitDef.requiredWorkTypes) ||
                a_pawn.WorkTagIsDisabled(newTraitDef.requiredWorkTags) || newTraitDef.forcedPassions != null &&
                a_pawn.workSettings != null && newTraitDef.forcedPassions.Any(p =>
                    p.IsDisabled(a_pawn.story.DisabledWorkTagsBackstoryAndTraits, a_pawn.GetDisabledWorkTypes(true))))
            {
                continue;
            }

            var degree = PawnGenerator.RandomTraitDegree(newTraitDef);
            if (a_pawn.story.Childhood != null && a_pawn.story.Childhood.DisallowsTrait(newTraitDef, degree) ||
                a_pawn.story.Adulthood != null && a_pawn.story.Adulthood.DisallowsTrait(newTraitDef, degree))
            {
                continue;
            }

            var trait = new Trait(newTraitDef, degree);
            if (a_pawn.mindState?.mentalBreaker == null ||
                !((a_pawn.mindState.mentalBreaker.BreakThresholdMinor +
                   trait.OffsetOfStat(StatDefOf.MentalBreakThreshold)) *
                    trait.MultiplierOfStat(StatDefOf.MentalBreakThreshold) > 0.5f))
            {
                a_pawn.story.traits.GainTrait(trait);
            }
        }
    }

    private static void GenerateSkills(Pawn a_pawn)
    {
        var allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
        foreach (var skillDef in allDefsListForReading)
        {
            var num = FinalLevelOfSkill(a_pawn, skillDef);
            var skill = a_pawn.skills.GetSkill(skillDef);
            skill.Level = num;
            if (skill.TotallyDisabled)
            {
                continue;
            }

            if (skill.def == SkillDefOf.Social)
            {
                skill.passion = Passion.Major;
                continue;
            }

            var num2 = num * 0.11f;
            var value = Rand.Value;
            if (value < num2)
            {
                skill.passion = value < num2 * 0.2f ? Passion.Major : Passion.Minor;
            }

            skill.xpSinceLastLevel = Rand.Range(skill.XpRequiredForLevelUp * 0.1f, skill.XpRequiredForLevelUp * 0.9f);
        }
    }

    private static int FinalLevelOfSkill(Pawn a_pawn, SkillDef a_sk)
    {
        float num = Rand.RangeInclusive(0, 4);
        foreach (var item in a_pawn.story.AllBackstories.Where(bs => bs != null))
        {
            foreach (var item2 in item.skillGains)
            {
                if (item2.skill == a_sk)
                {
                    num += item2.amount * Rand.Range(1f, 1.4f);
                }
            }
        }

        foreach (var trait in a_pawn.story.traits.allTraits)
        {
            if (trait.CurrentData.skillGains.Any(gain => gain.skill == a_sk))
            {
                num += trait.CurrentData.skillGains.First(gain => gain.skill == a_sk).amount;
            }
        }

        return Mathf.Clamp(Mathf.RoundToInt(num), 0, 20);
    }
}