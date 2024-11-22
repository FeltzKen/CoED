using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class StatusEffectManager : MonoBehaviour
    {
        [Header("Status Effect UI")]
        [SerializeField]
        private Transform statusEffectContainer;

        [SerializeField]
        private GameObject statusEffectIconPrefab;

        private List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();
        private Dictionary<StatusEffect, GameObject> effectIcons =
            new Dictionary<StatusEffect, GameObject>();

        private PlayerStats playerStats;
        private EnemyStats enemyStats; // apply effects for enemy combat

        // private EnemyPatrolChase enemyPatrolChase; // apply effects to enemy chase speed
        private FloatingTextManager floatingTextManager;

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            enemyStats = GetComponent<EnemyStats>();
            floatingTextManager = FloatingTextManager.Instance;
        }

        private void Update()
        {
            ApplyEffects();
            UpdateEffects();
        }

        private void ApplyEffects()
        {
            foreach (var activeEffect in activeEffects)
            {
                StatusEffect effect = activeEffect.Effect;
                float deltaTime = Time.deltaTime;

                if (effect.DamagePerSecond > 0)
                {
                    int damage = Mathf.CeilToInt(effect.DamagePerSecond * deltaTime);
                    if (playerStats != null)
                    {
                        playerStats.TakeDamage(damage);
                        Debug.Log($"Player taking damage: {damage}");
                        floatingTextManager?.ShowFloatingText(
                            damage.ToString(),
                            transform.position,
                            Color.red
                        );
                    }
                    else if (enemyStats != null)
                    {
                        enemyStats.TakeDamage(damage);
                        floatingTextManager?.ShowFloatingText(
                            damage.ToString(),
                            transform.position,
                            Color.red
                        );
                    }
                }

                if (effect.SpeedModifier != 0)
                {
                    if (playerStats != null)
                    {
                        playerStats.CurrentSpeed += effect.SpeedModifier * deltaTime; // Ensure CurrentSpeed is float
                    }
                    else if (enemyStats != null)
                    {
                        enemyStats.CurrentSpeed += effect.SpeedModifier * deltaTime; // Ensure patrolSpeed has public setter
                    }
                }

                if (effect.DefenseModifier != 0)
                {
                    if (playerStats != null)
                    {
                        /*73*/playerStats.CurrentDefense += effect.DefenseModifier * deltaTime; // Ensure CurrentDefense is float
                    }
                    else if (enemyStats != null)
                    {
                        /*77*/enemyStats.CurrentDefense += effect.DefenseModifier * deltaTime; // Ensure CurrentDefense is float
                    }
                }
            }
        }

        private void UpdateEffects()
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                ActiveStatusEffect activeEffect = activeEffects[i];
                activeEffect.RemainingDuration -= Time.deltaTime;

                if (activeEffect.RemainingDuration <= 0)
                {
                    RemoveEffect(activeEffect.Effect);
                }
            }
        }

        public void AddStatusEffect(StatusEffect effect)
        {
            if (effect == null)
            {
                Debug.LogWarning("StatusEffectManager: Attempted to add a null status effect.");
                return;
            }

            foreach (var activeEffect in activeEffects)
            {
                if (activeEffect.Effect.EffectName == effect.EffectName)
                {
                    activeEffect.RemainingDuration = effect.Duration;
                    Debug.Log(
                        $"StatusEffectManager: Refreshed duration for existing effect '{effect.EffectName}'."
                    );
                    return;
                }
            }

            activeEffects.Add(new ActiveStatusEffect(effect));
            DisplayEffectIcon(effect);
            Debug.Log($"StatusEffectManager: Added new status effect '{effect.EffectName}'.");
        }

        private void RemoveEffect(StatusEffect effect)
        {
            if (effect == null)
            {
                Debug.LogWarning("StatusEffectManager: Attempted to remove a null status effect.");
                return;
            }

            activeEffects.RemoveAll(ae => ae.Effect.EffectName == effect.EffectName);
            RemoveEffectIcon(effect);
            Debug.Log($"StatusEffectManager: Removed status effect '{effect.EffectName}'.");
        }

        private void DisplayEffectIcon(StatusEffect effect)
        {
            if (statusEffectIconPrefab != null && statusEffectContainer != null)
            {
                GameObject iconObj = Instantiate(statusEffectIconPrefab, statusEffectContainer);
                Image iconImage = iconObj.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = effect.Icon;
                }

                effectIcons[effect] = iconObj;
            }
            else
            {
                Debug.LogWarning(
                    "StatusEffectManager: StatusEffectIconPrefab or StatusEffectContainer is not assigned."
                );
            }
        }

        private void RemoveEffectIcon(StatusEffect effect)
        {
            if (effectIcons.TryGetValue(effect, out GameObject iconObj))
            {
                Destroy(iconObj);
                effectIcons.Remove(effect);
            }
        }

        private class ActiveStatusEffect
        {
            public StatusEffect Effect { get; private set; }
            public float RemainingDuration { get; set; }

            public ActiveStatusEffect(StatusEffect effect)
            {
                Effect = effect;
                RemainingDuration = effect.Duration;
            }
        }
    }
}
