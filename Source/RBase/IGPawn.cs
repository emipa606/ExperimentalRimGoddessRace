namespace RimGoddess.Base;

public interface IGPawn
{
    bool CanCast(float a_cost);

    void AddAbility(IGoddessAbilityDef a_def);

    void RemoveAbility(IGoddessAbilityDef a_def);

    bool SubtractFaith(float a_amount);

    bool AddReserveFaith(float a_amount);
}