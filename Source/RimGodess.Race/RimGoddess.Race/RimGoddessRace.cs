using RimGoddess.Base;
using Verse;

namespace RimGoddess.Race;

public class RimGoddessRace : Mod
{
    public static RimGoddessRace Instance;

    public static ModContentPack EquipmentMod;

    public RimGoddessRace(ModContentPack a_modContentPack)
        : base(a_modContentPack)
    {
        Instance = this;
        EquipmentMod = null;
        foreach (var runningMod in LoadedModManager.RunningMods)
        {
            switch (runningMod.PackageId.ToLower())
            {
                case "bladeofdebt.rimgoddess.equipmentex":
                case "bladeofdebt.rimgoddess.equipment":
                case "bladeofdebt.rimgodess.equipment[test]":
                    EquipmentMod = runningMod;
                    Log.Message("RimGoddess - Race: RimGoddess - Equipment found, using equipment defs");
                    goto end_IL_006d;
            }

            continue;
            end_IL_006d:
            break;
        }

        if (GPawnGenerators.GoddessGenerator == null)
        {
            GPawnGenerators.GoddessGenerator = new GoddessGenerator();
            Log.Message("RimGoddess - Race: Assigned Goddess Generator to default");
        }

        if (GPawnGenerators.HandmaidenGenerator == null)
        {
            GPawnGenerators.HandmaidenGenerator = new GoddessGenerator();
            Log.Message("RimGoddess - Race: Assigned Handmaiden Generator to default");
        }

        Log.Message("RimGoddess - Race: Loaded");
    }
}