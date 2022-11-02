using System;
using System.Collections.Generic;
using RimGoddess.Base;
using RimWorld;
using UnityEngine;
using Verse;
using StatDefOf = RimGoddess.Base.StatDefOf;

namespace RimGoddess.Race;

public class GoddessAbilityDef : Def, IGoddessAbilityDef
{
    public Type abilityClass = typeof(GoddessAbility);

    public KeyBindingDef hotKey;

    public string iconPath;

    public bool requiresTarget = true;

    public List<StatModifier> statBases;

    public Texture2D uiIcon = BaseContent.BadTex;

    public VerbProperties verbProperties;

    public float EffectRadius => statBases.GetStatValueFromList(StatDefOf.RTN_Stat_EffectRadius, 0f);

    public float EffectDuration => statBases.GetStatValueFromList(StatDefOf.RTN_Stat_EffectDuration, 0f);

    public bool HasAreaOfEffect => EffectRadius > float.Epsilon;

    public IEnumerable<string> StatSummary
    {
        get
        {
            if (FaithCost > 0f)
            {
                yield return string.Concat("RTN_Translation_FaithCost".Translate() + ": ", FaithCost.ToString());
            }

            if (verbProperties.warmupTime > 0f)
            {
                yield return string.Concat("AbilityCastingTime".Translate() + ": ",
                    verbProperties.warmupTime.ToString());
            }

            if (EffectDuration > 0f)
            {
                yield return string.Concat("AbilityDuration".Translate() + ": ", EffectDuration.ToString()) +
                             "LetterSecond".Translate();
            }

            if (HasAreaOfEffect)
            {
                yield return string.Concat("AbilityEffectRadius".Translate() + ": ",
                    Mathf.Ceil(EffectRadius).ToString());
            }
        }
    }

    public string Label => label;

    public VerbProperties VerbProperties => verbProperties;

    public bool RequiresTarget => requiresTarget;

    public float FaithCost => statBases.GetStatValueFromList(StatDefOf.RTN_Stat_FaithCost, 0f);

    public new string LabelCap => base.LabelCap;

    public Texture2D UiIcon => uiIcon;

    public KeyBindingDef HotKey => hotKey;

    public string GetTooltip()
    {
        return $"{LabelCap}\n\n{description}\n\n{StatSummary.ToLineList()}";
    }

    public override void PostLoad()
    {
        if (!string.IsNullOrEmpty(iconPath))
        {
            LongEventHandler.ExecuteWhenFinished(delegate { uiIcon = ContentFinder<Texture2D>.Get(iconPath); });
        }
    }

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (var item in base.ConfigErrors())
        {
            yield return item;
        }

        if (abilityClass == null)
        {
            yield return "abilityClass == null";
        }

        if (verbProperties == null)
        {
            yield return "verbProperties == null";
        }

        if (string.IsNullOrEmpty(label))
        {
            yield return "no label";
        }

        if (statBases == null)
        {
            yield break;
        }

        foreach (var statBase in statBases)
        {
            if (statBases.Count(st => st.stat == statBase.stat) > 1)
            {
                yield return $"defines the stat base {statBase.stat} more than once.";
            }
        }
    }
}