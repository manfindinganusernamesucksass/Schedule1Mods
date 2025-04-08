using HarmonyLib;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Persistence;
using MelonLoader;
using ShelfReservedSpace.DevUtils;

namespace ShelfReservedSpace.Patches.ObjectScripts;

public class StationWriteDataPatches
{
    [HarmonyPatch(typeof(BrickPress),
        nameof(BrickPress.WriteData))]
    [HarmonyPostfix]
    public static void BrickPressWriteDataPostfix(BrickPress __instance, 
        object[] __args, ref Il2CppSystem.Collections.Generic.List<string> __result)
        => SaveStationFilterData(__instance.Cast<ISaveable>(), (string)__args[0], 
            __instance.GUID, __instance.InputSlots, __result);

    [HarmonyPatch(typeof(Cauldron),
        nameof(Cauldron.WriteData))]
    [HarmonyPostfix]
    public static void CauldronWriteDataPostfix(Cauldron __instance, 
        object[] __args, ref Il2CppSystem.Collections.Generic.List<string> __result)
        => SaveStationFilterData(__instance.Cast<ISaveable>(), (string)__args[0], 
            __instance.GUID, __instance.InputSlots, __result);

    [HarmonyPatch(typeof(ChemistryStation),
        nameof(ChemistryStation.WriteData))]
    [HarmonyPostfix]
    public static void ChemistryStationWriteDataPostfix(ChemistryStation __instance, 
        object[] __args, ref Il2CppSystem.Collections.Generic.List<string> __result)
        => SaveStationFilterData(__instance.Cast<ISaveable>(), (string)__args[0], 
            __instance.GUID, __instance.InputSlots, __result);

    [HarmonyPatch(typeof(DryingRack),
        nameof(DryingRack.WriteData))]
    [HarmonyPostfix]
    public static void DryingRackWriteDataPostfix(DryingRack __instance, 
        object[] __args, ref Il2CppSystem.Collections.Generic.List<string> __result)
        => SaveStationFilterData(__instance.Cast<ISaveable>(), (string)__args[0], 
            __instance.GUID, __instance.InputSlots, __result);

    [HarmonyPatch(typeof(LabOven),
        nameof(LabOven.WriteData))]
    [HarmonyPostfix]
    public static void LabOvenWriteDataPostfix(LabOven __instance, 
        object[] __args, ref Il2CppSystem.Collections.Generic.List<string> __result)
        => SaveStationFilterData(__instance.Cast<ISaveable>(), (string)__args[0], 
            __instance.GUID, __instance.InputSlots, __result);

    [HarmonyPatch(typeof(MixingStation),
        nameof(MixingStation.WriteData))]
    [HarmonyPostfix]
    public static void MixingStationWriteDataPostfix(MixingStation __instance, 
        object[] __args, ref Il2CppSystem.Collections.Generic.List<string> __result)
        => SaveStationFilterData(__instance.Cast<ISaveable>(), (string)__args[0], 
            __instance.GUID, __instance.InputSlots, __result);

    [HarmonyPatch(typeof(PackagingStation),
        nameof(PackagingStation.WriteData))]
    [HarmonyPostfix]
    public static void PackagingStationWriteDataPostfix(PackagingStation __instance, 
        object[] __args, ref Il2CppSystem.Collections.Generic.List<string> __result)
        => SaveStationFilterData(__instance.Cast<ISaveable>(), (string)__args[0], 
            __instance.GUID, __instance.InputSlots, __result);

    private static void SaveStationFilterData(ISaveable saveable, string parentFolderPath,
        Il2CppSystem.Guid guid, Il2CppSystem.Collections.Generic.List<ItemSlot> slots,
        Il2CppSystem.Collections.Generic.List<string> result)
    {
        try
        {
            var filterInfo = new JObject();
            foreach (var slot in slots)
            {
                var filter = DevTools.GetShelfReserverSpaceFilter(slot);
                if (filter is not null)
                {
                    filterInfo.Add(slot.SlotIndex.ToString(), 
                        filter.FilterItem.GetItemData().GetJson());
                }
            }

            if (filterInfo.Count == 0)
            {
                return;
            }

            var jObject = new JObject();
            jObject.Add("GUID", guid);
            jObject.Add(DevTools.FILTERS_PROP_NAME, filterInfo);

            saveable.WriteSubfile(parentFolderPath, DevTools.FILTERS_PROP_NAME,
                jObject.ToString());
            result.Add($"{DevTools.FILTERS_PROP_NAME}.json");
        }
        catch (Exception ex)
        {
            Melon<ShelfReservedSpaceMod>.Logger.Error(
                $"Fucked up saving data in {parentFolderPath}", ex);
        }
    }
}
