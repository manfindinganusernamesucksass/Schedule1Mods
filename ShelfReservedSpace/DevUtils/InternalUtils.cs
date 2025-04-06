using Il2CppScheduleOne.ItemFramework;
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
}