using CoED;
using UnityEngine;

public class EquipmentPickup : MonoBehaviour
{
    [SerializeField]
    private Equipment baseEquipment;

    // The ScriptableObject asset you want to drop (e.g., LeatherHood.asset)

    private EquipmentWrapper wrapperData;

    private void Awake()
    {
        // 1) Create a new wrapper
        wrapperData = ScriptableObject.CreateInstance<EquipmentWrapper>();

        // 2) Initialize it with the base ScriptableObject data
        wrapperData.Initialize(baseEquipment);
    }

    public void SetData(EquipmentWrapper wrapper)
    {
        wrapperData = wrapper;
    }

    // This is what the Player calls in CollectItems()
    public EquipmentWrapper GetEquipmentData()
    {
        return wrapperData;
    }
}
