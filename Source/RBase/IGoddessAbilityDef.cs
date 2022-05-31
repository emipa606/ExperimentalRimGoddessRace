using UnityEngine;
using Verse;

namespace RimGoddess.Base;

public interface IGoddessAbilityDef
{
    string Label { get; }

    VerbProperties VerbProperties { get; }

    string LabelCap { get; }

    Texture2D UiIcon { get; }

    KeyBindingDef HotKey { get; }

    float FaithCost { get; }

    bool RequiresTarget { get; }

    string GetTooltip();
}