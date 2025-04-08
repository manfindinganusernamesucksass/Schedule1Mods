using Il2CppNewtonsoft.Json.Linq;
using Il2CppScheduleOne.ItemFramework;
using ShelfReservedSpace.InjectClasses;

namespace ShelfReservedSpace.DevUtils;

public static class DevTools
{
    public const string FILTERS_PROP_NAME = "Filters";

    public static ItemFilter_ShelfReservedSpace? GetShelfReserverSpaceFilter(
        ItemSlot itemSlot)
    {
        foreach (var filter in itemSlot.Filters)
        {
            if (filter is ItemFilter_ShelfReservedSpace targetFilter)
            {
                return targetFilter;
            }
        }

        return null;
    }

    public static Il2CppSystem.Collections.Generic.Dictionary<int, string>? GetFiltersFromJson(
        JObject jObject)
    {
        if (!jObject.ContainsKey(FILTERS_PROP_NAME))
        {
            return null;
        }

        var filters = jObject[FILTERS_PROP_NAME]
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
}