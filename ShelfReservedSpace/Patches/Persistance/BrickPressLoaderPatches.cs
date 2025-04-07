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

public class BrickPressLoaderPatches
{
    [HarmonyPatch(typeof(BrickPressLoader),
    nameof(BrickPressLoader.Load))]
    [HarmonyPostfix]
    public static void LoadPostfix(BrickPressLoader __instance, object[] __args)
    {
        try
		{
            var mainPath = (string)__args[0];
            if (!File.Exists(Il2CppSystem.IO.Path.Combine(mainPath, "Filters.json")) 
                || !__instance.TryLoadFile(mainPath, "Filters", out var text))
            {
                return;
            }

            var jobject = JObject.Parse(text);

            var filters = InternalUtils.GetFiltersFromJson(jobject);
            if (filters is null)
            {
                return;
            }

            var brickPress = GUIDManager
                .GetObject<GridItem>(Il2CppSystem.Guid.Parse((string)jobject["GUID"]))?
                .TryCast<BrickPress>()
                ?? throw new Exception("Entity not found");

            foreach (var slot in brickPress.InputSlots)
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
