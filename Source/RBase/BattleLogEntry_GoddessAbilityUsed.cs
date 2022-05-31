using Verse;
using Verse.Grammar;

namespace RimGoddess.Base;

public class BattleLogEntry_GoddessAbilityUsed : BattleLogEntry_Event
{
    public IGoddessAbilityDef goddessAbilityUsed;

    public BattleLogEntry_GoddessAbilityUsed()
    {
    }

    public BattleLogEntry_GoddessAbilityUsed(Pawn a_caster, Thing a_target, IGoddessAbilityDef a_ability,
        RulePackDef a_eventDef)
        : base(a_target, a_eventDef, a_caster)
    {
        goddessAbilityUsed = a_ability;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        var value = goddessAbilityUsed as Def;
        Scribe_Defs.Look(ref value, "goddessAbilityUsed");
        goddessAbilityUsed = value as IGoddessAbilityDef;
    }

    protected override GrammarRequest GenerateGrammarRequest()
    {
        var result = base.GenerateGrammarRequest();
        result.Rules.AddRange(GrammarUtility.RulesForDef("ABILITY", goddessAbilityUsed as Def));
        if (subjectPawn == null && subjectThing == null)
        {
            result.Rules.Add(new Rule_String("SUBJECT_definite", "AreaLower".Translate()));
        }

        return result;
    }
}