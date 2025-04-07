using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Items;
using MelonLoader;
using MelonLoader.Utils;
using ShelfReservedSpace.DevUtils;
using ShelfReservedSpace.InjectClasses;
using ShelfReservedSpace.Patches.ObjectScripts;
using ShelfReservedSpace.Patches.Persistance;
using ShelfReservedSpace.Patches.UI;
using Unity.Collections;
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
            if (Input.GetKeyDown(_filterSwitchKeyCode))
            {
                var hoveredItemSlotUi = ItemUIManager.Instance.GetHoveredItemSlot();
                var hoveredItemSlot = hoveredItemSlotUi?.assignedSlot;
                if (hoveredItemSlotUi is null || hoveredItemSlot is null
                    || !CanSlotHaveShelfReservedSpaceFilter(hoveredItemSlot))
                {
                    return;
                }

                var existingFilter = InternalUtils.GetShelfReserverSpaceFilter(
                    hoveredItemSlot);

                if (existingFilter is not null)
                {
                    Melon<ShelfReservedSpaceMod>.Logger.Msg("Removing filter.");

                    hoveredItemSlot.Filters.Remove(existingFilter);

                    ItemSlotUIPatches.DeactivateFilterImage(hoveredItemSlotUi);
                }
                else if (hoveredItemSlot.ItemInstance is not null)
                {
                    Melon<ShelfReservedSpaceMod>.Logger.Msg($"Adding filter for " +
                        $"{hoveredItemSlot.ItemInstance.Definition.ID}.");

                    hoveredItemSlot.AddFilter(
                        new ItemFilter_ShelfReservedSpace(
                            hoveredItemSlot.ItemInstance));

                    ItemSlotUIPatches.ActivateFilterImage(hoveredItemSlotUi,
                        hoveredItemSlot.ItemInstance.Icon);
                }
            }
        }
        catch (Exception ex)
        {
            Melon<ShelfReservedSpaceMod>.Logger.Error(
                "Error on user input.", ex);
        }
    }

    private static bool CanSlotHaveShelfReservedSpaceFilter(ItemSlot slot)
    {
        try
        {
            if (StorageMenu.Instance.OpenedStorageEntity is not null
                && StorageMenu.Instance.OpenedStorageEntity.ItemSlots.Contains(slot))
            {
                return StorageMenu.Instance.OpenedStorageEntity.StorageEntityName.Contains("Storage Rack");
            }
            else if (slot.SlotOwner is null)
            {
                return false;
            }
            else if (slot.SlotOwner.TryCast<BrickPress>() is BrickPress brickPress)
            {
                return brickPress.InputSlots.Contains(slot);
            }
            else if (slot.SlotOwner.TryCast<Cauldron>() is Cauldron cauldron)
            {
                return cauldron.InputSlots.Contains(slot);
            }
            else if (slot.SlotOwner.TryCast<ChemistryStation>() is ChemistryStation chemistryStation)
            {
                return chemistryStation.InputSlots.Contains(slot);
            }
            else if (slot.SlotOwner.TryCast<DryingRack>() is DryingRack rack)
            {
                return rack.InputSlots.Contains(slot);
            }
            else if (slot.SlotOwner.TryCast<LabOven>() is LabOven labOven)
            {
                return labOven.InputSlots.Contains(slot);
            }
            // MixingStationMk2 inherits from MixingStation
            else if (slot.SlotOwner.TryCast<MixingStation>() is MixingStation mixingStation)
            {
                return mixingStation.InputSlots.Contains(slot);
            }
            else if (slot.SlotOwner.TryCast<PackagingStation>() is PackagingStation packagingStation)
            {
                return packagingStation.InputSlots.Contains(slot);
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Melon<ShelfReservedSpaceMod>.Logger.Error(
                "Error trying to determine if filter can be added to the slot.",
                ex);
            return false;
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
        harmony.PatchAll(typeof(BrickPressPatches));
        harmony.PatchAll(typeof(BrickPressLoaderPatches));
    }
}