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

        [SerializeField]
        private Transform enemyStatusEffectContainer;

        [SerializeField]
        private StatusEffectIconLibrary iconLibrary;

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
            //ApplyEffects(playerEffects);
            //ApplyEffects(enemyEffects);

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
                rectTransform.localScale = Vector3.one; // Ensure correct scaling
                //rectTransform.sizeDelta = new Vector2(1, 1); // Set size to match icons
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

        private void ApplyEffects(Dictionary<GameObject, List<ActiveStatusEffect>> targetEffects)
        {
            foreach (var entityEntry in targetEffects)
            {
                GameObject entity = entityEntry.Key;
                List<ActiveStatusEffect> effects = entityEntry.Value;

                foreach (var activeEffect in effects)
                {
                    StatusEffect effect = activeEffect.Effect;

                    // Dynamically apply effects based on entity type
                    if (entity.CompareTag("Player"))
                    {
                        var playerStats = entity.GetComponent<PlayerStats>();
                        if (playerStats != null)
                        {
                            effect.ApplyToEntity(entity); // Let StatusEffect handle the logic
                        }
                    }
                    else if (entity.CompareTag("Enemy"))
                    {
                        var enemyStats = entity.GetComponent<EnemyStats>();
                        if (enemyStats != null)
                        {
                            effect.ApplyToEntity(entity); // Let StatusEffect handle the logic
                        }
                    }
                }
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
                    if (activeEffect.Effect.hasDuration)
                    {
                        activeEffect.RemainingDuration -= Time.deltaTime;

                        if (activeEffect.RemainingDuration <= 0)
                        {
                            RemoveEffect(entity, activeEffect.Effect);
                        }
                    }
                }
            }
        }

        private void RemoveEffect(GameObject entity, StatusEffect effect)
        {
            bool isPlayer = playerEffects.ContainsKey(entity);
            var targetEffects = isPlayer ? playerEffects : enemyEffects;

            if (!targetEffects.ContainsKey(entity))
                return;

            targetEffects[entity].RemoveAll(ae => ae.Effect.effectName == effect.effectName);
            RemoveEffectIcon(effect, isPlayer);
            Debug.Log($"Removed status effect '{effect.effectName}' from {entity.name}.");
        }

        private void RemoveEffectIcon(StatusEffect effect, bool isPlayer)
        {
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
