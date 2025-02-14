using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class StatusEffectManager : MonoBehaviour
    {
        public static StatusEffectManager Instance { get; private set; }

        // This dictionary tracks active status effects on entities.
        // Key: entity GameObject; Value: List of active StatusEffect objects.
        private Dictionary<GameObject, List<StatusEffect>> activeEffects =
            new Dictionary<GameObject, List<StatusEffect>>();

        //public StatusEffectLibrary statusEffectLibrary; // Reference set in Inspector

        // Map certain StatusEffectTypes to the Immunities they represent.
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
                // Add additional mappings as needed.
            };

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            // Update each active effect.
            float deltaTime = Time.deltaTime;
            List<GameObject> entitiesToClean = new List<GameObject>();

            foreach (var kvp in activeEffects)
            {
                GameObject entity = kvp.Key;
                List<StatusEffect> effects = kvp.Value;
                IEntityStats entityStats = entity.GetComponent<IEntityStats>();
                if (entityStats == null)
                    continue;

                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    StatusEffect effect = effects[i];
                    effect.UpdateEffect(deltaTime, entityStats);

                    if (effect.hasDuration && effect.duration <= 0f)
                    {
                        effect.Cleanup(entityStats);
                        effects.RemoveAt(i);
                    }
                }
                if (effects.Count == 0)
                    entitiesToClean.Add(entity);
            }

            foreach (GameObject entity in entitiesToClean)
            {
                activeEffects.Remove(entity);
            }
        }

        /// <summary>
        /// Adds Effects from Equipment to the entity.
        /// </summary>
        public void AddEffectsFromEquipment(GameObject entity, List<StatusEffectType> effects)
        {
            foreach (StatusEffectType effect in effects)
            {
                AddStatusEffect(entity, effect, -1f);
            }
        }

        /// <summary>
        /// Removes Effects from Equipment to the entity.
        /// </summary>
        public void RemoveEquipmentEffects(GameObject entity, List<StatusEffectType> effects)
        {
            foreach (StatusEffectType effect in effects)
            {
                RemoveStatusEffect(entity, effect);
            }
        }

        /// <summary>
        /// Applies (or refreshes) a status effect on the given entity.
        /// </summary>
        public void AddStatusEffect(GameObject entity, StatusEffectType effectType, float duration)
        {
            IEntityStats entityStats = entity.GetComponent<IEntityStats>();
            if (entityStats == null)
            {
                Debug.LogError($"{entity.name} does not have an IEntityStats component.");
                return;
            }

            // Check immunity.
            if (effectToImmunityMap.TryGetValue(effectType, out Immunities requiredImmunity))
            {
                if (
                    entityStats is IHasImmunities immunizedEntity
                    && immunizedEntity.HasImmunity(requiredImmunity)
                )
                {
                    Debug.Log($"{entity.name} is immune to {effectType}.");
                    return;
                }
            }

            // Ensure a list exists.
            if (!activeEffects.ContainsKey(entity))
                activeEffects[entity] = new List<StatusEffect>();

            // If effect already exists, refresh duration.
            foreach (StatusEffect existingEffect in activeEffects[entity])
            {
                if (existingEffect.effectType == effectType)
                {
                    existingEffect.duration = duration;
                    // NotifySubscriber(entity, activeEffects[entity]);
                    return;
                }
            }

            // Otherwise, clone a new instance from the library.
            StatusEffect definition = StatusEffectLibrary.GetDefinition(effectType);
            if (definition != null)
            {
                StatusEffect newEffect = definition.Clone();
                // Use a version of Initialize that sets tickDamage, tickInterval, periodic flag, hasDuration flag, and custom duration.
                newEffect.Initialize(
                    newEffect.tickDamageOrHeal,
                    newEffect.effectIconPrefab,
                    newEffect.tickInterval,
                    newEffect.hasPeriodicTick,
                    newEffect.hasDuration,
                    duration
                );
                activeEffects[entity].Add(newEffect);
                entityStats.AddActiveStatusEffect(effectType);
                newEffect.ApplyToEntity(entityStats, newEffect.tickDamageOrHeal);
                Debug.Log($"{entity.name} is now affected by {effectType}.");
                NotifySubscriber(entity, activeEffects[entity]);
            }
            else
            {
                Debug.LogError($"No definition found for status effect: {effectType}");
            }
        }

        /// <summary>
        /// Removes a specific status effect from an entity.
        /// </summary>
        public void RemoveStatusEffect(GameObject entity, StatusEffectType effectType)
        {
            if (!activeEffects.ContainsKey(entity))
                return;

            IEntityStats entityStats = entity.GetComponent<IEntityStats>();
            List<StatusEffect> effects = activeEffects[entity];
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                if (effects[i].effectType == effectType)
                {
                    effects[i].Cleanup(entityStats);
                    effects.RemoveAt(i);
                    entityStats.RemoveActiveStatusEffect(effectType);
                }
            }
            NotifySubscriber(entity, effects);
            if (effects.Count == 0)
                activeEffects.Remove(entity);
        }

        /// <summary>
        /// Removes all status effects from an entity.
        /// </summary>
        public void RemoveAllStatusEffects(GameObject entity)
        {
            if (!activeEffects.ContainsKey(entity))
                return;

            IEntityStats entityStats = entity.GetComponent<IEntityStats>();
            foreach (StatusEffect effect in activeEffects[entity])
            {
                effect.Cleanup(entityStats);
                entityStats.RemoveActiveStatusEffect(effect.effectType);
            }
            activeEffects.Remove(entity);
            NotifySubscriber(entity, new List<StatusEffect>());
        }

        /// <summary>
        /// Notifies the status effect subscriber (if any) on the given entity.
        /// </summary>
        private void NotifySubscriber(GameObject entity, List<StatusEffect> effects)
        {
            // Look for a component that implements IStatusEffectSubscriber on the entity or its children.
            IStatusEffectSubscriber subscriber = entity.GetComponentInChildren<IStatusEffectSubscriber>();
            if (subscriber != null)
            {
                subscriber.OnStatusEffectsChanged(effects);
            }
        }
    }
}
