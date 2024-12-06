using System;
using UnityEngine;

namespace CoED
{
    [System.Serializable]
    public class StatusEffect : MonoBehaviour
    {
        [Header("Effect Details")]
        [SerializeField]
        private string effectName;

        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private float duration;

        [SerializeField]
        private float damagePerSecond;

        [SerializeField]
        private float speedModifier;

        [SerializeField]
        private float defenseModifier;

        private float elapsedTime;

        public string EffectName => effectName;
        public Sprite Icon => icon;
        public float Duration => duration;
        public float DamagePerSecond => damagePerSecond;
        public float SpeedModifier => speedModifier;
        public float DefenseModifier => defenseModifier;
        public bool IsExpired => elapsedTime >= duration;

        // Constructor to initialize status effect properties
        public StatusEffect(
            string name,
            float duration,
            float damagePerSecond,
            Sprite icon = null,
            float speedModifier = 0,
            float defenseModifier = 0
        )
        {
            this.effectName = name;
            this.duration = duration;
            this.damagePerSecond = damagePerSecond;
            this.icon = icon;
            this.speedModifier = speedModifier;
            this.defenseModifier = defenseModifier;
            elapsedTime = 0f; // Initialize elapsed time to 0
        }

        public void ApplyEffect(MonoBehaviour target)
        {
            if (IsExpired)
                return; // Prevent applying if the effect has expired

            elapsedTime += Time.deltaTime;

            if (damagePerSecond > 0)
            {
                int damage = Mathf.CeilToInt(damagePerSecond * Time.deltaTime);
                PlayerStats.Instance.TakeDamage(damage);
                // Debug.Log($"Player taking damage: {damage}");
             //   FloatingTextManager.Instance?.ShowFloatingText( damage.ToString(),target.transform,Color.red );              
            }

            if (target is PlayerStats playerStats)
            {
                playerStats.CurrentSpeed += speedModifier; // Ensure CurrentSpeed is float
                playerStats.CurrentDefense += (int)defenseModifier; // Ensure CurrentDefense is float
            }
            else if (target is Enemy enemy) // Change to Enemy
            {
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>(); // Access the EnemyAI component for stats

                if (enemyStats != null) // Ensure it's not null
                {
                    enemyStats.CurrentSpeed += Mathf.RoundToInt(speedModifier); // Modify the enemy's current speed
                    enemyStats.CurrentDefense += Mathf.RoundToInt(defenseModifier); // Modify the enemy's current defense
                }
            }
        }

        public void ResetEffect()
        {
            elapsedTime = 0f; // Reset elapsed time when effect is applied or refreshed
        }
    }
}
