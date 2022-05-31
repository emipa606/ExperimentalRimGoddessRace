using System;
using System.Collections.Generic;
using RimGoddess.Base;
using Verse;

namespace RimGoddess.Race;

public class PawnGoddessAbilityController : IExposable
{
    private readonly GoddessPawn m_pawn;
    private List<GoddessAbility> m_abilities;

    public PawnGoddessAbilityController(GoddessPawn a_pawn)
    {
        m_pawn = a_pawn;
        m_abilities = new List<GoddessAbility>();
    }

    public void ExposeData()
    {
        Scribe_Collections.Look(ref m_abilities, "goddessAbilities", LookMode.Deep, m_pawn);
        if (Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }

        foreach (var ability in m_abilities)
        {
            if (ability.def == null)
            {
                m_abilities.Remove(ability);
            }
        }
    }

    public void AddAbility(GoddessAbilityDef a_def)
    {
        foreach (var ability in m_abilities)
        {
            if (ability.def == a_def)
            {
                return;
            }
        }

        m_abilities.Add(Activator.CreateInstance(a_def.abilityClass, m_pawn, a_def) as GoddessAbility);
    }

    public void RemoveAbility(GoddessAbilityDef a_def)
    {
        foreach (var ability in m_abilities)
        {
            if (ability.def != a_def)
            {
                continue;
            }

            m_abilities.Remove(ability);
            break;
        }
    }

    public IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var ability in m_abilities)
        {
            yield return ability.GetGizmo();
        }
    }

    internal void AbilityControllerTick()
    {
        foreach (var goddessAbility in m_abilities)
        {
            goddessAbility.AbilityTick();
        }
    }
}