// CurseRemover.cs
using UnityEngine;
using CoED;

namespace CoED
{
    /// Handles removing curses from items.
    public static class CurseRemover
    {
        /// Attempts to remove the curse from the given item.
        public static void UseOnEquipment(Equipment equipment)
        {
            if (equipment == null)
            {
                Debug.LogWarning("CurseRemover: Attempted to use on a null item.");
                return;
            }

            if (equipment.IsCursed)
            {
                equipment.RemoveCurse();
                // Debug.Log($"{equipment.equipmentName} has been uncursed.");

                // Optionally, we could invoke some UI or status update here.
                // For example: UIManager.Instance?.UpdateInventoryDisplay();
            }
            else
            {
                // Debug.Log("CurseRemover: This item is not cursed or cannot be uncursed.");
            }
        }
    }
}
