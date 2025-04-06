using HarmonyLib;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppScheduleOne.EntityFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.Persistence.Datas;
using Il2CppScheduleOne.Persistence.Loaders;
using MelonLoader;
using ShelfReservedSpace.InjectClasses;
using UnityEngine;

namespace ShelfReservedSpace.Patches;

public class StorageRackLoaderPatches
{
    /// <summary>
    /// Override the original <see cref="StorageRackLoader.Load(string)"/>
    /// to try to obtain the <see cref="ItemFilter_ShelfReservedSpace"/>
    /// stored by <see cref="PlaceableStorageEntityPatches"/>.
    /// </summary>
    [HarmonyPatch(typeof(StorageRackLoader),
        nameof(StorageRackLoader.Load))]
    [HarmonyPrefix]
    public static bool Prefix(StorageRackLoader __instance, object[] __args)
    {
        try
        {
            var path = (string)__args[0];

            GridItem gridItem = __instance.LoadAndCreate(path)
                ?? throw new Exception("Grid item null");

            PlaceableStorageEntity? placeableStorageEntity =
                gridItem.TryCast<PlaceableStorageEntity>()
                ?? throw new Exception("Placeable storage null");

            PlaceableStorageData? data =
                CustomGetData(__instance, path, out var filters)
                ?? throw new Exception("Data not found");

            for (int i = 0; i < data.Contents.Items.Length; i++)
            {
                if (placeableStorageEntity.StorageEntity.ItemSlots.Count <= i)
                {
                    break;
                }

                var slot = placeableStorageEntity.StorageEntity.ItemSlots[i];

                slot.SetStoredItem(ItemDeserializer.LoadItem(data.Contents.Items[i]), false);

                if (filters.TryGetValue(slot.SlotIndex.ToString(), out var filterData))
                {
                    slot.AddFilter(
                        new ItemFilter_ShelfReservedSpace(
                            ItemDeserializer.LoadItem(filterData)));
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            Melon<ShelfReservedSpaceMod>.Logger.Error(
                $"Messed up loading filter data for {__args.FirstOrDefault()}.",
                ex);
            return true;
        }
    }

    /// <summary>
    /// Tried to use a custom type extending <see cref="PlaceableStorageData"/> with
    /// the filters' data, but for some reason <see cref="BuildableItemLoader.GetData{T}(string)"/>
    /// wouldn't get the filters' data, so I'm stuck with this.
    /// </summary>
    private static PlaceableStorageData? CustomGetData(StorageRackLoader loader, string mainPath,
        out Il2CppSystem.Collections.Generic.Dictionary<string, string> filters)
    {
        filters = new Il2CppSystem.Collections.Generic.Dictionary<string, string>();
        if (loader.TryLoadFile(mainPath, "Data", out string text))
        {
            var data = JsonUtility.FromJson<PlaceableStorageData>(text);
            if (data is null)
            {
                return null;
            }

            if (text.Contains(PlaceableStorageEntityPatches.FILTERS_PROPERTY_NAME))
            {
                try
                {
                    filters = JObject.Parse(text)[PlaceableStorageEntityPatches.FILTERS_PROPERTY_NAME]
                        .ToObject<Il2CppSystem.Collections.Generic.Dictionary<string, string>>();
                }
                catch (Exception ex)
                {
                    Melon<ShelfReservedSpaceMod>.Logger.Error(
                        "Messed up getting filters from json", ex);
                }
            }

            return data;
        }

        return null;
    }
}