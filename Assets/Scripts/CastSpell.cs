using UnityEngine;
using System.Collections.Generic;

namespace CoED
{
    public class CastSpell : MonoBehaviour
    {
        [Header("Spell Settings")]
        [SerializeField] private List<Spell> availableSpells;
        [SerializeField] private Transform spellSpawnPoint;

        private PlayerStats playerStats;
        private int selectedSpellIndex = 0;

        private void Start()
        {
            playerStats = GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("CastSpell: PlayerStats component is missing on the player object.");
                return;
            }

            // Add a default Fireball spell if no spells are available for testing
            if (availableSpells.Count == 0)
            {
                availableSpells.Add(CreateDefaultFireball());
                // Debug.Log("CastSpell: Added default Fireball spell for testing.");
            }
        }

        public void CastSelectedSpell(Vector3 targetPosition)
        {
            if (availableSpells.Count == 0) return;

            Spell selectedSpell = availableSpells[selectedSpellIndex];

            // Check if player has enough MP to cast the spell
            if (playerStats.CurrentMagic < selectedSpell.MagicCost)
            {
                // Debug.Log($"Not enough MP to cast {selectedSpell.SpellName}.");
                return;
            }

            // Consume MP and cast the spell
            playerStats.CurrentMagic -= selectedSpell.MagicCost;
            // Debug.Log($"Casting spell: {selectedSpell.SpellName}");

            // Handle spell type
            switch (selectedSpell.Type)
            {
                case SpellType.Projectile:
                    CastProjectileSpell(selectedSpell, targetPosition);
                    break;

                case SpellType.AoE:
                    CastAoESpell(selectedSpell);
                    break;

                case SpellType.Heal:
                    CastHealSpell(selectedSpell);
                    break;
            }
        }

        // Casts a projectile spell
        private void CastProjectileSpell(Spell spell, Vector3 targetPosition)
        {
            if (ProjectileManager.Instance == null)
            {
                Debug.LogError("CastSpell: ProjectileManager instance is missing.");
                return;
            }

            ProjectileManager.Instance.LaunchProjectile(spellSpawnPoint.position, targetPosition);
        }

        // Casts an AoE spell at the player's position
        private void CastAoESpell(Spell spell)
        {
            // Debug.Log($"Casting AoE spell: {spell.SpellName}");
            spell.Cast(transform, spellSpawnPoint.position);
        }

        // Casts a heal spell on the player
        private void CastHealSpell(Spell spell)
        {
            // Debug.Log($"Casting Heal spell: {spell.SpellName}");
            playerStats.Heal(spell.Damage);
        }

        public void SelectSpell(int spellIndex)
        {
            if (spellIndex >= 0 && spellIndex < availableSpells.Count)
            {
                selectedSpellIndex = spellIndex;
                // Debug.Log($"Selected spell: {availableSpells[selectedSpellIndex].SpellName}");
            }
            else
            {
                Debug.LogWarning("Invalid spell index selected.");
            }
        }

        // Creates a default Fireball spell if none are available for testing
        private Spell CreateDefaultFireball()
        {
            Spell fireball = ScriptableObject.CreateInstance<Spell>();
            fireball.SetSpellData("Fireball", null, 10, 25, 1.0f, SpellType.Projectile, 10f, 0f, /* Assign Fireball prefab here */ null);
            return fireball;
        }
    }

    // Enum to define spell types
    public enum SpellType
    {
        Projectile,
        AoE,
        Heal,
    }
}
