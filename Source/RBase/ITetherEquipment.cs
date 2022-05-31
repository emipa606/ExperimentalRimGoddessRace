using Verse;

namespace RimGoddess.Base;

public interface ITetherEquipment
{
    bool Tethered { get; }

    float TetherFaithDraw { get; }

    void Notify_Tethered(Pawn a_pawn, bool a_value);
}