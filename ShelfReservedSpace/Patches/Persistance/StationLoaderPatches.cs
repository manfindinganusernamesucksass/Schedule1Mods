using HarmonyLib;
using Il2Cpp;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppScheduleOne.EntityFramework;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.Persistence.Loaders;
using MelonLoader;
using ShelfReservedSpace.DevUtils;
using ShelfReservedSpace.InjectClasses;

namespace ShelfReservedSpace.Patches.Persistance;

public class StationLoaderPatches
{
    [HarmonyPatch(typeof(BrickPressLoader),
    nameof(BrickPressLoader.Load))]
    [HarmonyPostfix]
    public static void BrickPressLoaderLoadPostfix(BrickPressLoader __instance, object[] __args)
        => LoadData<BrickPress>(__instance, (string)__args[0], s => s.InputSlots);

    [HarmonyPatch(typeof(CauldronLoader),
    nameof(CauldronLoader.Load))]
    [HarmonyPostfix]
    public static void CauldronLoaderLoadPostfix(CauldronLoader __instance, object[] __args)
        => LoadData<Cauldron>(__instance, (string)__args[0], s => s.InputSlots);

    [HarmonyPatch(typeof(ChemistryStationLoader),
    nameof(ChemistryStationLoader.Load))]
    [HarmonyPostfix]
    public static void ChemistryStationLoaderLoadPostfix(ChemistryStationLoader __instance, object[] __args)
        => LoadData<ChemistryStation>(__instance, (string)__args[0], s => s.InputSlots);

    [HarmonyPatch(typeof(DryingRackLoader),
    nameof(DryingRackLoader.Load))]
    [HarmonyPostfix]
    public static void DryingRackLoaderLoadPostfix(DryingRackLoader __instance, object[] __args)
        => LoadData<DryingRack>(__instance, (string)__args[0], s => s.InputSlots);

    [HarmonyPatch(typeof(LabOvenLoader),
    nameof(LabOvenLoader.Load))]
    [HarmonyPostfix]
    public static void LabOvenLoaderLoadPostfix(LabOvenLoader __instance, object[] __args)
        => LoadData<LabOven>(__instance, (string)__args[0], s => s.InputSlots);

    [HarmonyPatch(typeof(MixingStationLoader),
    nameof(MixingStationLoader.Load))]
    [HarmonyPostfix]
    public static void MixingStationLoaderLoadPostfix(MixingStationLoader __instance, object[] __args)
        => LoadData<MixingStation>(__instance, (string)__args[0], s => s.InputSlots);

    [HarmonyPatch(typeof(PackagingStationLoader),
    nameof(PackagingStationLoader.Load))]
    [HarmonyPostfix]
    public static void PackagingStationLoaderLoadPostfix(PackagingStationLoader __instance, object[] __args)
        => LoadData<PackagingStation>(__instance, (string)__args[0], s => s.InputSlots);

    private static void LoadData<T>(GridItemLoader loader, string mainPath, 
        Func<T, Il2CppSystem.Collections.Generic.List<ItemSlot>> getInputSlots)
        where T : GridItem
    {
        try
        {
            if (!File.Exists(Il2CppSystem.IO.Path.Combine(mainPath, $"{DevTools.FILTERS_PROP_NAME}.json"))
                || !loader.TryLoadFile(mainPath, DevTools.FILTERS_PROP_NAME, out var text))
            {
                return;
            }

            var jobject = JObject.Parse(text);

            var filters = DevTools.GetFiltersFromJson(jobject);
            if (filters is null)
            {
                return;
            }

            var station = GUIDManager
                .GetObject<GridItem>(Il2CppSystem.Guid.Parse((string)jobject["GUID"]))?
                .TryCast<T>()
                ?? throw new Exception("Entity not found");

            var slots = getInputSlots(station);

            foreach (var slot in slots)
            {
                if (filters.TryGetValue(slot.SlotIndex, 
                    out var filterData))
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
