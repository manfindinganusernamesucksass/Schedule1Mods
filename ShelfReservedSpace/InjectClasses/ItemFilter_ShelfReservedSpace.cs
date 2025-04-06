using Il2CppInterop.Runtime.Injection;
using Il2CppScheduleOne.ItemFramework;
using MelonLoader;

namespace ShelfReservedSpace.InjectClasses;

/// <summary>
/// <see cref="ItemFilter"/> implementation to only
/// allow items with the same item definition.
/// </summary>
/// <remarks>
/// An <see cref="ItemInstance"/> is used instead of 
/// <see cref="ItemDefinition"/> because that's 
/// what the game uses to save the data, so we can reuse that.
/// </remarks>
[RegisterTypeInIl2Cpp]
public class ItemFilter_ShelfReservedSpace : ItemFilter
{
    public ItemInstance FilterItem { get; init; } = null!;

    public ItemFilter_ShelfReservedSpace(IntPtr ptr) : base(ptr) { }

    public ItemFilter_ShelfReservedSpace(ItemInstance filterItem)
        : base(ClassInjector.DerivedConstructorPointer<ItemFilter_ShelfReservedSpace>())
    {
        FilterItem = filterItem;
        ClassInjector.DerivedConstructorBody(this);
    }

    public override bool DoesItemMatchFilter(ItemInstance instance)
        => FilterItem.Definition.ID == instance.Definition.ID;
}