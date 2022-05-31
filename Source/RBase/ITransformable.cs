using UnityEngine;
using Verse;

namespace RimGoddess.Base;

public interface ITransformable
{
    void Notify_Transformed(IGPawn a_pawn, bool a_value, Color a_baseColor);

    void Notify_ShaderRefresh(Pawn a_pawn);
}