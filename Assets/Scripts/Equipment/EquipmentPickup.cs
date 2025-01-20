using CoED;
using UnityEngine;

public class EquipmentPickup : MonoBehaviour
{
    private Equipment equipmentData;

    public void SetEquipment(Equipment equipment)
    {
        equipmentData = equipment;

        // Show only the base sprite (affix hidden)
        GetComponent<SpriteRenderer>().sprite = equipmentData.baseSprite;
    }

    public Equipment GetEquipmentData()
    {
        return equipmentData;
    }
}
