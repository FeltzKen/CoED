using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoED
{
    public class StatusEffectManager : MonoBehaviour
    {
        public static StatusEffectManager Instance { get; private set; }

        [Header("Status Effect UI")]
        [SerializeField]
        private Transform playerStatusEffectContainer;

        private Dictionary<GameObject, List<ActiveStatusEffect>> playerEffects =
            new Dictionary<GameObject, List<ActiveStatusEffect>>();
        private Dictionary<GameObject, List<ActiveStatusEffect>> enemyEffects =
            new Dictionary<GameObject, List<ActiveStatusEffect>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            UpdateEffects(playerEffects);
            UpdateEffects(enemyEffects);
        }

        public void AddStatusEffect(GameObject entity, StatusEffect effectPrefab)
        {
            bool isPlayer = entity.CompareTag("Player");
            var targetEffects = isPlayer ? playerEffects : enemyEffects;

            Transform targetContainer;

            // Determine the appropriate container
            if (isPlayer)
            {
                targetContainer = playerStatusEffectContainer;
            }
            else
            {
                // Dynamically find the StatusEffectsPanel for the enemy
                var enemyPanel = entity.transform.Find("StatusEffectsPanel");
                if (enemyPanel == null)
                {
                    Debug.LogWarning($"Enemy {entity.name} does not have a StatusEffectsPanel.");
                    return;
                }
                targetContainer = enemyPanel;
            }

            // Add the entity to the dictionary if not already present
            if (!targetEffects.ContainsKey(entity))
            {
                targetEffects[entity] = new List<ActiveStatusEffect>();
            }

            List<ActiveStatusEffect> effects = targetEffects[entity];

            // Check for duplicate effects
            foreach (var activeEffect in effects)
            {
                if (activeEffect.Effect.effectType == effectPrefab.effectType)
                {
                    Debug.LogWarning(
                        $"Effect {effectPrefab.effectType} already applied to {entity.name}. Refreshing duration."
                    );
                    activeEffect.RemainingDuration = effectPrefab.duration;
                    return;
                }
            }

            // Instantiate the effect prefab
            GameObject effectObj = Instantiate(effectPrefab.gameObject);
            effectObj.transform.SetParent(targetContainer, false); // Assign to the correct container
            effectObj.SetActive(true); // Ensure the instantiated prefab is active

            StatusEffect newEffect = effectObj.GetComponent<StatusEffect>();
            // Adjust RectTransform for layout group
            RectTransform rectTransform = effectObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
            }
            else
            {
                Debug.LogWarning("StatusEffectManager: Icon prefab missing RectTransform.");
            }
            if (newEffect != null)
            {
                newEffect.ApplyToEntity(entity); // Apply the effect to the entity
                effects.Add(new ActiveStatusEffect(newEffect));
                Debug.Log($"Applied {newEffect.effectType} to {entity.name}.");
            }
            else
            {
                Debug.LogWarning("StatusEffectManager: Failed to apply status effect.");
            }
        }

        private void UpdateEffects(Dictionary<GameObject, List<ActiveStatusEffect>> targetEffects)
        {
            foreach (var entityEntry in targetEffects)
            {
                GameObject entity = entityEntry.Key;
                List<ActiveStatusEffect> effects = entityEntry.Value;

                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    ActiveStatusEffect activeEffect = effects[i];

                    // Null Safety Check
                    if (
                        entity == null
                        || activeEffect.Effect == null
                        || activeEffect.Effect.gameObject == null
                    )
                    {
                        effects.RemoveAt(i);
                        continue;
                    }

                    // Update Effect Duration
                    activeEffect.RemainingDuration -= Time.deltaTime;

                    if (activeEffect.RemainingDuration <= 0)
                    {
                        RemoveEffect(entity, activeEffect.Effect);
                    }
                }
            }
        }

        public void RemoveAllDebuffStatusEffects(GameObject entity)
        {
            RemoveEffectsByType(
                entity,
                new StatusEffectType[]
                {
                    StatusEffectType.Burn,
                    StatusEffectType.Poison,
                    StatusEffectType.Stun,
                    StatusEffectType.Slow,
                }
            );
        }

        public void RemoveAllBuffStatusEffects(GameObject entity)
        {
            RemoveEffectsByType(
                entity,
                new StatusEffectType[]
                {
                    StatusEffectType.Regen,
                    StatusEffectType.Shield,
                    StatusEffectType.Invincible,
                }
            );
        }

        public void RemoveAllStatusEffects(GameObject entity)
        {
            RemoveEffectsByType(
                entity,
                System.Enum.GetValues(typeof(StatusEffectType)) as StatusEffectType[]
            );
        }

        private void RemoveEffectsByType(GameObject entity, StatusEffectType[] effectTypes)
        {
            bool isPlayer = playerEffects.ContainsKey(entity);
            var targetEffects = isPlayer ? playerEffects : enemyEffects;

            if (!targetEffects.ContainsKey(entity))
            {
                Debug.LogWarning($"Entity {entity.name} does not have any effects to remove.");
                return;
            }

            List<ActiveStatusEffect> effects = targetEffects[entity];

            foreach (var effectType in effectTypes)
            {
                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    ActiveStatusEffect activeEffect = effects[i];
                    if (activeEffect.Effect.effectType == effectType)
                    {
                        RemoveEffect(entity, activeEffect.Effect);
                        Debug.Log($"Removed {effectType} from {entity.name}.");
                    }
                }
            }
        }

        public void RemoveSpecificEffect(GameObject entity, StatusEffectType effectType)
        {
            bool isPlayer = playerEffects.ContainsKey(entity);
            var targetEffects = isPlayer ? playerEffects : enemyEffects;

            if (!targetEffects.ContainsKey(entity))
            {
                Debug.LogWarning($"Entity {entity.name} does not have any effects to remove.");
                return;
            }

            List<ActiveStatusEffect> effects = targetEffects[entity];

            for (int i = effects.Count - 1; i >= 0; i--)
            {
                ActiveStatusEffect activeEffect = effects[i];
                if (activeEffect.Effect.effectType == effectType)
                {
                    RemoveEffect(entity, activeEffect.Effect);
                    Debug.Log($"Removed {effectType} from {entity.name}.");
                    return;
                }
            }

            Debug.LogWarning($"Effect {effectType} not found on {entity.name}.");
        }

        public void RemoveEffect(GameObject entity, StatusEffect effect)
        {
            bool isPlayer = playerEffects.ContainsKey(entity);
            var targetEffects = isPlayer ? playerEffects : enemyEffects;

            if (!targetEffects.ContainsKey(entity))
                return;

            targetEffects[entity].RemoveAll(ae => ae.Effect == effect);
            RemoveEffectIcon(effect, isPlayer);
            Debug.Log($"Removed status effect '{effect.effectName}' from {entity.name}.");
        }

        private void RemoveEffectIcon(StatusEffect effect, bool isPlayer)
        {
            if (effect == null || effect.gameObject == null)
            {
                Debug.LogWarning(
                    "StatusEffectManager: Attempted to remove a null or already destroyed effect."
                );
                return;
            }

            // Destroy the effect's game object
            Destroy(effect.gameObject);
        }

        private class ActiveStatusEffect
        {
            public StatusEffect Effect { get; private set; }
            public float RemainingDuration { get; set; }

            public ActiveStatusEffect(StatusEffect effect)
            {
                Effect = effect;
                RemainingDuration = effect.duration;
            }
        }
    }
}
