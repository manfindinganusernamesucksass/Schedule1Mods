using HarmonyLib;
using Il2CppNewtonsoft.Json;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Persistence;
using MelonLoader;
using ShelfReservedSpace.DevUtils;

namespace ShelfReservedSpace.Patches.ObjectScripts;

public class BrickPressPatches
{
    [HarmonyPatch(typeof(BrickPress),
        nameof(BrickPress.WriteData))]
    [HarmonyPostfix]
    public static void WriteDataPostfix(BrickPress __instance, object[] __args, ref Il2CppSystem.Collections.Generic.List<string> __result)
    {
        try
        {
            if (InternalUtils.TrySaveStationFilterData(
                __instance.Cast<ISaveable>(), (string)__args[0],
                __instance.GUID, __instance.InputSlots))
            {
                __result.Add("Filters.json");
            }
        }
        catch (Exception ex)
        {
            Melon<ShelfReservedSpaceMod>.Logger.Error("fuck", ex);
        }
    }
}
