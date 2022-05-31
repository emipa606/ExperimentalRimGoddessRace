using RimWorld;

namespace RimGoddess.Base;

public interface IGPawnGenerator
{
    IGPawn GenerateGPawn(Faction a_faction, int a_tile, IGPawn a_baseGoddess = null);
}