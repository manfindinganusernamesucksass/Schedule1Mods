using HarmonyLib;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppScheduleOne.ObjectScripts;
using MelonLoader;
using ShelfReservedSpace.DevUtils;

namespace ShelfReservedSpace.Patches.ObjectScripts;

public class PlaceableStorageEntityPatches
{
    public const string FILTERS_PROPERTY_NAME = "Filters";

    /// <summary>
    /// If any of the storage's slots has a 
    /// <see cref="InjectClasses.ItemFilter_ShelfReservedSpace"/>,
    /// add the details to the save string to retrieve them on load.
    /// </summary>
    [HarmonyPatch(typeof(PlaceableStorageEntity),
        nameof(PlaceableStorageEntity.GetSaveString))]
    [HarmonyPostfix]
    public static void GetSaveStringPostfix(PlaceableStorageEntity __instance, ref string __result)
    {
        var original = __result;

        try
        {
            var filters = new JObject();
            foreach (var slot in __instance.StorageEntity.ItemSlots)
            {
                var existingFilter = InternalUtils.GetShelfReserverSpaceFilter(slot);
                if (existingFilter is not null)
                {
                    filters.Add(slot.SlotIndex.ToString(),
                        existingFilter.FilterItem.GetItemData().GetJson());
                }
            }

            if (filters.Count > 0)
            {
                var data = JObject.Parse(__result);
                data.Add(FILTERS_PROPERTY_NAME, filters);
                __result = data.ToString();
            }
        }
        catch (Exception ex)
        {
            Melon<ShelfReservedSpaceMod>.Logger.Error(
                "Error saving storage's filter data. Restoring original.", ex);
            __result = original;
        }
    }
}