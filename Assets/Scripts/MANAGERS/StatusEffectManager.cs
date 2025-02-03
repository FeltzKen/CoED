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
        private Dictionary<GameObject, List<StatusEffect>> activeEffects =
            new Dictionary<GameObject, List<StatusEffect>>();

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
                { StatusEffectType.Blindness, Immunities.Shadow },
                { StatusEffectType.Silence, Immunities.Arcane },
                { StatusEffectType.Curse, Immunities.Shadow },
                { StatusEffectType.Bleed, Immunities.Physical },
                { StatusEffectType.Confusion, Immunities.Arcane },
                { StatusEffectType.Fear, Immunities.Shadow },
                { StatusEffectType.Petrify, Immunities.Arcane },
                { StatusEffectType.Root, Immunities.Nature },
                { StatusEffectType.Sleep, Immunities.Arcane },
                { StatusEffectType.Berserk, Immunities.Physical },
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
            UpdateEffects();
        }

        /// <summary>
        /// Spawns the status effect prefab and attaches it to the entity.
        /// </summary>
        public void AddStatusEffect(GameObject entity, StatusEffectType effectType, float duration)
        {
            var entityStats = entity.GetComponent<IEntityStats>();
            if (entityStats == null)
            {
                Debug.LogError($"AddStatusEffect: {entity.name} has no IEntityStats component.");
                return;
            }

            if (!activeEffects.ContainsKey(entity))
                activeEffects[entity] = new List<StatusEffect>();

            foreach (var existingEffect in activeEffects[entity])
            {
                if (existingEffect.effectType == effectType)
                {
                    existingEffect.duration = duration; // Refresh duration
                    return;
                }
            }
        }

        /// <summary>
        /// Main loop to decrement durations and remove expired effects.
        /// </summary>
        private void UpdateEffects()
        {
            foreach (var entityEffects in activeEffects)
            {
                List<StatusEffect> effects = entityEffects.Value;
                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    StatusEffect effect = effects[i];
                    if (!effect.hasDuration)
                        continue;

                    effect.duration -= Time.deltaTime;
                    if (effect.duration <= 0)
                    {
                        RemoveStatusEffect(entityEffects.Key, effect);
                    }
                }
            }
        }

        public void RemoveStatusEffect(GameObject entity, StatusEffect effect)
        {
            if (!activeEffects.ContainsKey(entity))
                return;

            activeEffects[entity].Remove(effect);
            Destroy(effect.gameObject);
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
                    StatusEffectType.RandomDebuff,
                    StatusEffectType.Blindness,
                    StatusEffectType.Silence,
                    StatusEffectType.Curse,
                    StatusEffectType.Bleed,
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
