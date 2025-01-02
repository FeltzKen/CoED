using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    [CreateAssetMenu(
        fileName = "StatusEffectIconLibrary",
        menuName = "ScriptableObjects/StatusEffectIconLibrary"
    )]
    public class StatusEffectIconLibrary : ScriptableObject
    {
        [Serializable]
        public class StatusEffectIconEntry
        {
            public StatusEffectType EffectType;
            public GameObject EffectPrefab;
        }

        public List<StatusEffectIconEntry> EffectPrefabs = new List<StatusEffectIconEntry>();

        public List<GameObject> GetEffectPrefabs()
        {
            List<GameObject> effectPrefabs = new List<GameObject>();
            foreach (var entry in EffectPrefabs)
            {
                effectPrefabs.Add(entry.EffectPrefab);
            }
            return effectPrefabs;
        }

        public GameObject GetEffectPrefab(StatusEffectType effectType)
        {
            foreach (var entry in EffectPrefabs)
            {
                Debug.Log($"Checking entry: {entry.EffectType}");
                if (entry.EffectType == effectType)
                {
                    Debug.Log($"Found prefab for {effectType}");
                    return entry.EffectPrefab;
                }
            }
            Debug.LogWarning($"No prefab found for status effect type: {effectType}");
            return null;
        }
    }
}
