// CurseRemover.cs
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    /// Handles removing curses from items.
    public static class CurseRemover
    {
        /// Attempts to remove the curse from the given item.
        public static void UseOnItem(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("CurseRemover: Attempted to use on a null item.");
                return;
            }

            if (item.IsCursed)
            {
                item.RemoveCurse();
                Debug.Log($"{item.ItemName} has been uncursed.");

                // Optionally, we could invoke some UI or status update here.
                // For example: UIManager.Instance?.UpdateInventoryDisplay();
            }
            else
            {
                Debug.Log("CurseRemover: This item is not cursed or cannot be uncursed.");
            }
        }
    }
}
