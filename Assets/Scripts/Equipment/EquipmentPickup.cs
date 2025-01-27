using CoED;
using UnityEngine;

public class EquipmentPickup : MonoBehaviour
{
    private Equipment equipmentData;
    public bool isEnchantedOrCursed = false;

    [SerializeField]
    private string itemName;

    public void SetEquipment(Equipment equipment)
    {
        equipmentData = equipment;
        itemName = equipmentData.itemName;
        isEnchantedOrCursed = equipmentData.isEnchantedOrCursed;
        // Show only the base sprite (affix hidden)
        GetComponent<SpriteRenderer>().sprite = equipmentData.baseSprite;
    }

    public Equipment GetEquipmentData()
    {
        return equipmentData;
    }
}
