using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Items;
using MelonLoader;
using MelonLoader.Utils;
using ShelfReservedSpace.DevUtils;
using ShelfReservedSpace.InjectClasses;
using ShelfReservedSpace.Patches;
using UnityEngine;
using Path = System.IO.Path;

namespace ShelfReservedSpace;

public class ShelfReservedSpaceMod : MelonMod
{
    private KeyCode _filterSwitchKeyCode = KeyCode.B;

    public override void OnInitializeMelon()
    {
        SetupMelonPreferences();
        PatchHarmony();
    }

    public override void OnLateUpdate()
    {
        try
        {
            if (Input.GetKeyDown(_filterSwitchKeyCode)
                && StorageMenu.Instance.OpenedStorageEntity is not null)
            {
                var hoveredSlot = ItemUIManager.Instance.GetHoveredItemSlot();
                if (hoveredSlot?.assignedSlot is null
                    || !StorageMenu.Instance.OpenedStorageEntity.ItemSlots.Contains(hoveredSlot.assignedSlot))
                {
                    return;
                }

                var existingFilter = InternalUtils.GetShelfReserverSpaceFilter(
                    hoveredSlot.assignedSlot);

                if (existingFilter is not null)
                {
                    Melon<ShelfReservedSpaceMod>.Logger.Msg("Removing filter.");

                    hoveredSlot.assignedSlot.Filters.Remove(existingFilter);

                    ItemSlotUIPatches.DeactivateFilterImage(hoveredSlot);
                }
                else if (hoveredSlot.assignedSlot.ItemInstance is not null)
                {
                    Melon<ShelfReservedSpaceMod>.Logger.Msg($"Adding filter for " +
                        $"{hoveredSlot.assignedSlot.ItemInstance.Definition.ID}.");

                    hoveredSlot.assignedSlot.AddFilter(
                        new ItemFilter_ShelfReservedSpace(
                            hoveredSlot.assignedSlot.ItemInstance));

                    ItemSlotUIPatches.ActivateFilterImage(hoveredSlot,
                        hoveredSlot.assignedSlot.ItemInstance.Icon);
                }
            }
        }
        catch (Exception ex)
        {
            Melon<ShelfReservedSpaceMod>.Logger.Error(
                "Error on user input.", ex);
        }
    }

    private void SetupMelonPreferences()
    {
        string preferencesFolder = Path.Combine(
            MelonEnvironment.UserDataDirectory, nameof(ShelfReservedSpace));

        Directory.CreateDirectory(preferencesFolder);

        var category = MelonPreferences.CreateCategory("Shelf reserved space patch");
        category.SetFilePath(Path.Combine(preferencesFolder, "Preferences.cfg"));

        var keyCode = category.CreateEntry("Bind", KeyCode.B.ToString());
        _filterSwitchKeyCode = Enum.TryParse<KeyCode>(keyCode.Value, true, out var result)
            ? result : KeyCode.B;
        Melon<ShelfReservedSpaceMod>.Logger.Msg(
            $"Key binded for this mod: '{_filterSwitchKeyCode}'.");
    }

    private static void PatchHarmony()
    {
        var harmony = new HarmonyLib.Harmony(nameof(ShelfReservedSpaceMod));
        harmony.PatchAll(typeof(ItemSlotUIPatches));
        harmony.PatchAll(typeof(PlaceableStorageEntityPatches));
        harmony.PatchAll(typeof(StorageRackLoaderPatches));
    }
}