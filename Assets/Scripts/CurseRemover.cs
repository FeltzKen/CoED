// CurseRemover.cs
using UnityEngine;

namespace CoED
{
    public static class CurseRemover
    {
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
                FloatingTextManager.Instance.ShowFloatingText(
                    "Curse removed!",
                    PlayerMovement.Instance.transform,
                    Color.green
                );
            }
            else
            {
                FloatingTextManager.Instance.ShowFloatingText(
                    "This item is not cursed or cannot be uncursed.",
                    PlayerMovement.Instance.transform,
                    Color.red
                );
            }
        }
    }
}
