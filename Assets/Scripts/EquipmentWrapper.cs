using UnityEngine;

namespace CoED
{
    public class EquipmentWrapper : MonoBehaviour
    {
        [Header("Equipment Data")]
        public Equipment equipmentData;

        public void Initialize(Equipment data)
        {
            equipmentData = Instantiate(data);
        }

        public void ApplyStatModifiers(float scaledFactor)
        {
            if (equipmentData == null)
            {
                Debug.LogError("EquipmentWrapper: Equipment data is missing!");
                return;
            }

            if (equipmentData.attackModifier > 0)
                equipmentData.attackModifier += scaledFactor;
            if (equipmentData.defenseModifier > 0)
                equipmentData.defenseModifier += scaledFactor;
            if (equipmentData.healthModifier > 0)
                equipmentData.healthModifier += scaledFactor;
            if (equipmentData.speedModifier > 0)
                equipmentData.speedModifier += scaledFactor;
            if (equipmentData.magicModifier > 0)
                equipmentData.magicModifier += scaledFactor;
            if (equipmentData.staminaModifier > 0)
                equipmentData.staminaModifier += scaledFactor;
            Debug.Log(
                $"EquipmentWrapper: Applied stat modifiers with scale factor {scaledFactor} to {equipmentData.equipmentName}"
            );
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                CollectItem(PlayerActions.Instance);
            }
        }

        private void CollectItem(PlayerActions playerActions)
        {
            if (playerActions != null)
            {
                playerActions.CollectItem(this);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("ItemCollectible: PlayerActions is null.");
            }
        }
    }
}
