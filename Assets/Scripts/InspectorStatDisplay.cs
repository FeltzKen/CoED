using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    [ExecuteAlways]
    public class InspectorStatDisplay : MonoBehaviour
    {
        // Individual stat fields for inspector display.
        public float MaxHP;
        public float Attack;
        public float Intelligence;
        public float Evasion;
        public float Defense;
        public float Dexterity;
        public float Accuracy;
        public float MaxMagic;
        public float MaxStamina;
        public float Shield;
        public float FireRate;
        public float CritChance;
        public float CritDamage;
        public float ProjectileRange;
        public float AttackRange;
        public float Speed;
        public float ElementalDamage;
        public float ChanceToInflictStatusEffect;
        public float StatusEffectDuration;
        public float PatrolSpeed;
        public float ChaseSpeed;

        /// <summary>
        /// Copies the values from enemyStats into the individual fields.
        /// </summary>
        public void SetStats()
        {
            Monster enemyMonsterData = GetComponent<_EnemyStats>().monsterData;
            if (enemyMonsterData == null)
                return;

            //float value;
            MaxHP = enemyMonsterData.monsterStats[Stat.MaxHP];
            Attack = enemyMonsterData.monsterStats[Stat.Attack];
            Intelligence = enemyMonsterData.monsterStats[Stat.Intelligence];
            Evasion = enemyMonsterData.monsterStats[Stat.Evasion];
            Defense = enemyMonsterData.monsterStats[Stat.Defense];
            Dexterity = enemyMonsterData.monsterStats[Stat.Dexterity];
            Accuracy = enemyMonsterData.monsterStats[Stat.Accuracy];
            MaxMagic = enemyMonsterData.monsterStats[Stat.MaxMagic];
            MaxStamina = enemyMonsterData.monsterStats[Stat.MaxStamina];
            Shield = enemyMonsterData.monsterStats[Stat.Shield];
            FireRate = enemyMonsterData.monsterStats[Stat.FireRate];
            CritChance = enemyMonsterData.monsterStats[Stat.CritChance];
            CritDamage = enemyMonsterData.monsterStats[Stat.CritDamage];
            ProjectileRange = enemyMonsterData.monsterStats[Stat.ProjectileRange];
            AttackRange = enemyMonsterData.monsterStats[Stat.AttackRange];
            Speed = enemyMonsterData.monsterStats[Stat.Speed];
            ElementalDamage = enemyMonsterData.monsterStats[Stat.ElementalDamage];
            ChanceToInflictStatusEffect = enemyMonsterData.monsterStats[Stat.ChanceToInflictStatusEffect];
            StatusEffectDuration = enemyMonsterData.monsterStats[Stat.StatusEffectDuration];
            PatrolSpeed = enemyMonsterData.monsterStats[Stat.PatrolSpeed];
            ChaseSpeed = enemyMonsterData.monsterStats[Stat.ChaseSpeed];
        }
    }
}
