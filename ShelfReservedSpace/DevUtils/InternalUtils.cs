using Il2CppNewtonsoft.Json;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Persistence;
using ShelfReservedSpace.InjectClasses;

namespace ShelfReservedSpace.DevUtils;

public static class InternalUtils
{
    public static ItemFilter_ShelfReservedSpace? GetShelfReserverSpaceFilter(ItemSlot itemSlot)
    {
        foreach (var filter in itemSlot.Filters)
        {
            if (filter is ItemFilter_ShelfReservedSpace shelfReservedSpaceFilter)
            {
                return shelfReservedSpaceFilter;
            }
        }

        return null;
    }

    public static Il2CppSystem.Collections.Generic.Dictionary<int, string>? GetFiltersFromJson(JObject jObject)
    {
        if (!jObject.ContainsKey("Filters"))
        {
            return null;
        }

        var filters = jObject["Filters"]
            .ToObject<Il2CppSystem.Collections.Generic.Dictionary<string, string>>();

        if (filters is null)
        {
            return null;
        }

        var result = new Il2CppSystem.Collections.Generic.Dictionary<int, string>();
        foreach (var filter in filters)
        {
            if (int.TryParse(filter.Key, out var value))
            {
                result[value] = filter.Value;
            }
        }

        return result;
    }

    public static bool TrySaveStationFilterData(ISaveable saveable, string parentFolderPath,
        Il2CppSystem.Guid guid, Il2CppSystem.Collections.Generic.List<ItemSlot> slots)
    {
        var filterInfo = new JObject();
        foreach (var slot in slots)
        {
            var filter = GetShelfReserverSpaceFilter(slot);
            if (filter is not null)
            {
                filterInfo.Add(slot.SlotIndex.ToString(), filter.FilterItem.GetItemData().GetJson());
            }
        }

        if (filterInfo.Count == 0)
        {
            return false;
        }

        var jObject = new JObject();
        jObject.Add("GUID", guid);
        jObject.Add("Filters", filterInfo);

        saveable.WriteSubfile(parentFolderPath, "Filters", 
            jObject.ToString());
        return true;
    }
}