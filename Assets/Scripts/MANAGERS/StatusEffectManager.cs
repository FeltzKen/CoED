using System.Collections.Generic;
using UnityEngine;

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

        public StatusEffectIconLibrary statusEffectLibrary;

        // Map certain StatusEffectTypes to the elemental Resistances they represent.
        // If the player has Resistances.Fire, they’ll block Burn, etc.
        private static readonly Dictionary<StatusEffectType, Immunities> effectToImmunityMap =
            new Dictionary<StatusEffectType, Immunities>
            {
                { StatusEffectType.Burn, Immunities.Fire },
                { StatusEffectType.Poison, Immunities.Poison },
                { StatusEffectType.Freeze, Immunities.Ice },
                { StatusEffectType.Stun, Immunities.Lightning },
                { StatusEffectType.Blindness, Immunities.Darkness },
                // You can add Slow => Resistances.Ice, Stun => Resistances.Lightning, etc., if desired.
            };

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

        /// <summary>
        /// Spawns the status effect prefab and attaches it to the entity.
        /// </summary>
        public void AddStatusEffect(GameObject entity, StatusEffectType effectType)
        {
            bool isPlayer = entity.CompareTag("Player");
            var targetEffects = isPlayer ? playerEffects : enemyEffects;

            // -- 1) Check Resistances for Players
            if (isPlayer)
            {
                // If this status effect maps to an elemental damage type that the player resists:
                if (effectToImmunityMap.TryGetValue(effectType, out Immunities neededImmunity))
                {
                    if (PlayerStats.Instance.activeImmunities.Contains(neededImmunity))
                    {
                        Debug.Log($"Player is resistant to {effectType}; effect blocked.");
                        return;
                    }
                }
            }
            else
            {
                // -- 2) Resistance Check for Enemies
                _EnemyStats enemyStats = entity.GetComponent<_EnemyStats>();
                if (
                    enemyStats != null
                    && effectToImmunityMap.TryGetValue(effectType, out Immunities neededImmunity)
                )
                {
                    if (enemyStats.immunities.Contains(neededImmunity))
                    {
                        Debug.Log($"{entity.name} is resistant to {effectType}. Effect blocked.");
                        return;
                    }
                }
            }

            // Determine container (player or enemy)
            Transform targetContainer;
            if (isPlayer)
            {
                targetContainer = playerStatusEffectContainer; // The player UI slot
            }
            else
            {
                var enemyPanel = entity.transform.Find("StatusEffectsPanel");
                if (enemyPanel == null)
                {
                    Debug.LogWarning($"Enemy {entity.name} does not have a StatusEffectsPanel.");
                    return;
                }
                targetContainer = enemyPanel;
            }

            // Ensure entity is in the dictionary
            if (!targetEffects.ContainsKey(entity))
            {
                targetEffects[entity] = new List<ActiveStatusEffect>();
            }

            // Check for duplicate effect
            List<ActiveStatusEffect> effects = targetEffects[entity];
            foreach (var activeEffect in effects)
            {
                if (activeEffect.Effect != null && activeEffect.Effect.effectType == effectType)
                {
                    // Refresh duration
                    Debug.LogWarning(
                        $"Effect {effectType} already on {entity.name}; refreshing duration."
                    );
                    activeEffect.RemainingDuration = statusEffectLibrary
                        .GetEffectPrefab(effectType)
                        .GetComponent<StatusEffect>()
                        .duration;
                    return;
                }
            }

            // Instantiate the effect prefab
            GameObject effectObj = Instantiate(
                statusEffectLibrary.GetEffectPrefab(effectType),
                targetContainer
            );
            effectObj.SetActive(true);

            // Optionally scale the icon
            RectTransform rectTransform = effectObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one * 3.0f;
            }

            StatusEffect newEffect = effectObj.GetComponent<StatusEffect>();
            if (newEffect != null)
            {
                newEffect.ApplyToEntity(entity);
                effects.Add(new ActiveStatusEffect(newEffect));
                Debug.Log($"Applied {newEffect.effectType} to {entity.name}.");
            }
            else
            {
                Debug.LogWarning("StatusEffectManager: Prefab missing StatusEffect component.");
                Destroy(effectObj);
            }
        }

        /// <summary>
        /// Main loop to decrement durations and remove expired effects.
        /// </summary>
        private void UpdateEffects(Dictionary<GameObject, List<ActiveStatusEffect>> targetEffects)
        {
            foreach (var kvp in targetEffects)
            {
                GameObject entity = kvp.Key;
                List<ActiveStatusEffect> effects = kvp.Value;

                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    ActiveStatusEffect activeEffect = effects[i];
                    if (
                        entity == null
                        || activeEffect.Effect == null
                        || activeEffect.Effect.gameObject == null
                    )
                    {
                        effects.RemoveAt(i);
                        continue;
                    }

                    if (!activeEffect.Effect.hasDuration)
                        continue; // E.g. a permanent effect that doesn’t expire

                    activeEffect.RemainingDuration -= Time.deltaTime;
                    if (activeEffect.RemainingDuration <= 0)
                    {
                        RemoveEffect(entity, activeEffect.Effect);
                    }
                }
            }
        }

        // -------------- Public Removal Methods -----------------

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
                    StatusEffectType.Freeze,
                    StatusEffectType.Blindness,
                    StatusEffectType.Silence,
                    StatusEffectType.Curse,
                    StatusEffectType.Bleed,
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
                    StatusEffectType.PoisonAura,
                    StatusEffectType.StealHealth,
                }
            );
        }

        public void RemoveAllStatusEffects(GameObject entity)
        {
            // Remove everything
            RemoveEffectsByType(
                entity,
                System.Enum.GetValues(typeof(StatusEffectType)) as StatusEffectType[]
            );
        }

        public void RemoveSpecificEffect(GameObject entity, StatusEffectType effectType)
        {
            bool isPlayer = playerEffects.ContainsKey(entity);
            var targetEffects = isPlayer ? playerEffects : enemyEffects;

            if (!targetEffects.ContainsKey(entity))
            {
                Debug.LogWarning($"{entity.name} has no status effects to remove.");
                return;
            }

            List<ActiveStatusEffect> effects = targetEffects[entity];
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                if (effects[i].Effect.effectType == effectType)
                {
                    RemoveEffect(entity, effects[i].Effect);
                    return;
                }
            }
        }
        public void RemoveEquipmentEffects(ActiveWhileEquipped effectType)
        {
            if (playerEffects.Count == 0)
            {
                Debug.LogWarning("No player effects to remove.");
                return;
            }

        }

        // --------------- Internal Removal -----------------

        private void RemoveEffectsByType(GameObject entity, StatusEffectType[] effectTypes)
        {
            bool isPlayer = playerEffects.ContainsKey(entity);
            var targetEffects = isPlayer ? playerEffects : enemyEffects;

            if (!targetEffects.ContainsKey(entity))
                return;

            List<ActiveStatusEffect> effects = targetEffects[entity];
            foreach (var effectType in effectTypes)
            {
                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    if (effects[i].Effect.effectType == effectType)
                    {
                        RemoveEffect(entity, effects[i].Effect);
                    }
                }
            }
        }

        public void RemoveEffect(GameObject entity, StatusEffect effect)
        {
            bool isPlayer = playerEffects.ContainsKey(entity);
            var targetEffects = isPlayer ? playerEffects : enemyEffects;

            if (!targetEffects.ContainsKey(entity))
                return;

            targetEffects[entity].RemoveAll(ae => ae.Effect == effect);
            Destroy(effect.gameObject); // This triggers OnDestroy in StatusEffect
            Debug.Log($"Removed effect '{effect.effectName}' from {entity.name}.");
        }

        // ----------------------------------------------

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
