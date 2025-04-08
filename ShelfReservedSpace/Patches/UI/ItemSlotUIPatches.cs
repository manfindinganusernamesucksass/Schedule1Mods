using HarmonyLib;
using Il2CppGameKit.Utilities;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.UI;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ShelfReservedSpace.Patches.UI;

public class ItemSlotUIPatches
{
    const string FILTER_GO_NAME = "ShelfReservedSpace_SlotFilterImage";

    [HarmonyPatch(typeof(ItemSlotUI),
        nameof(ItemSlotUI.AssignSlot))]
    [HarmonyPostfix]
    public static void AssignSlotPostfix(ItemSlotUI __instance, object[] __args)
    {
        try
        {
            var itemSlot = (ItemSlot)__args[0];

            var slotFilter = DevUtils.DevTools.GetShelfReserverSpaceFilter(itemSlot);
            if (slotFilter is null)
            {
                DeactivateFilterImage(__instance);
            }
            else
            {
                ActivateFilterImage(__instance, slotFilter.FilterItem.Icon);
            }
        }
        catch (Exception ex)
        {
            Melon<ShelfReservedSpaceMod>.Logger.Error(
                "Error updating item slot UI.", ex);
        }
    }

    /// <summary>
    /// If a slot has a <see cref="InjectClasses.ItemFilter_ShelfReservedSpace"/>,
    /// update its UI to show the item icon, so the player knows that the slot
    /// is reserved, and what item is it reserved for.
    /// </summary>
    internal static void ActivateFilterImage(ItemSlotUI itemSlotUi, Sprite filterSprite)
    {
        var filterUiObj = GetFilterUiGameObject(itemSlotUi);

        if (filterUiObj is null)
        {
            filterUiObj = new GameObject(FILTER_GO_NAME);
            filterUiObj.transform.SetParent(itemSlotUi.gameObject.transform, false);
            filterUiObj.transform.SetScale(new Vector3(0.25f, 0.25f));
            filterUiObj.transform.localPosition = new Vector3(-30, 30);

            var filterUiObjChild = new GameObject("Image");
            filterUiObjChild.transform.SetParent(filterUiObj.transform, false);
            filterUiObjChild.AddComponent<CanvasRenderer>();
            filterUiObjChild.AddComponent<Image>();
        }

        filterUiObj.transform.GetChild(0).GetComponent<Image>().overrideSprite
            = filterSprite;
        filterUiObj.SetActive(true);
    }

    internal static void DeactivateFilterImage(ItemSlotUI itemSlotUi)
        => GetFilterUiGameObject(itemSlotUi)?.SetActive(false);

    private static GameObject? GetFilterUiGameObject(ItemSlotUI itemSlotUi)
    {
        for (int i = 0; i < itemSlotUi.gameObject.transform.childCount; i++)
        {
            var child = itemSlotUi.gameObject.transform.GetChild(i);
            if (child.name == FILTER_GO_NAME)
            {
                return child.gameObject;
            }
        }

        return null;
    }
}