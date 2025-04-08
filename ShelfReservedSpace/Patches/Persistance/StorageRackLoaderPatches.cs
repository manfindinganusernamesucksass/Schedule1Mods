using HarmonyLib;
using Il2Cpp;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppScheduleOne.EntityFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.Persistence.Loaders;
using MelonLoader;
using ShelfReservedSpace.DevUtils;
using ShelfReservedSpace.InjectClasses;

namespace ShelfReservedSpace.Patches.Persistance;

public class StorageRackLoaderPatches
{
    [HarmonyPatch(typeof(StorageRackLoader),
        nameof(StorageRackLoader.Load))]
    [HarmonyPostfix]
    public static void LoadPostfix(StorageRackLoader __instance, object[] __args)
    {
        try
        {
            if (!__instance.TryLoadFile((string)__args[0], "Data", out var text))
            {
                return;
            }

            var jobject = JObject.Parse(text);

            var filters = DevTools.GetFiltersFromJson(jobject);
            if (filters is null)
            {
                return;
            }

            var storageEntity = GUIDManager
                .GetObject<GridItem>(Il2CppSystem.Guid.Parse((string)jobject["GUID"]))?
                .TryCast<PlaceableStorageEntity>()
                ?? throw new Exception("Entity not found");

            foreach (var slot in storageEntity.StorageEntity.ItemSlots)
            {
                if (filters.TryGetValue(slot.SlotIndex, out var filterData))
                {
                    slot.AddFilter(
                        new ItemFilter_ShelfReservedSpace(
                            ItemDeserializer.LoadItem(filterData)));
                }
            }
        }
        catch (Exception ex)
        {
            Melon<ShelfReservedSpaceMod>.Logger.Error("fuck me", ex);
        }
    }
}