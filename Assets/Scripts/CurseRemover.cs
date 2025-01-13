// CurseRemover.cs
using UnityEngine;

namespace CoED
{
    public static class CurseRemover
    {
        public static void UseOnEquipment(EquipmentWrapper equipmentWrapper)
        {
            if (equipmentWrapper == null)
            {
                Debug.LogWarning("CurseRemover: Attempted to use on a null item.");
                return;
            }

            if (equipmentWrapper.IsCursed)
            {
                equipmentWrapper.RemoveCurse();
                FloatingTextManager.Instance.ShowFloatingText(
                    "Curse removed!",
                    PlayerStats.Instance.transform,
                    Color.green
                );
            }
            else
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "This item is not cursed or cannot be uncursed.",
                    PlayerStats.Instance.transform,
                    Color.red
                );
            }
        }
    }
}
