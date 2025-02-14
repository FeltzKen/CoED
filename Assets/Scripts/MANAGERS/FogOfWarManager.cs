using System.Collections.Generic;
using UnityEngine;

namespace CoED
{
    public class FogOfWarManager : MonoBehaviour
    {
        public static FogOfWarManager Instance { get; private set; }

        [Header("Fog Settings")]
        public float visionRange = 5f;

        [Header("Shader Material")]
        public Material fogAlphaMaterial; // Material that uses FogOfWarAlphaShader
        public Sprite oneByOneSprite; // A plain 1×1 white sprite

        private Dictionary<int, Texture2D> fogTextures = new Dictionary<int, Texture2D>();
        private Dictionary<int, GameObject> overlayObjects = new Dictionary<int, GameObject>();
        private Dictionary<int, Dictionary<Vector2Int, FogStatus>> floorFogStatus =
            new Dictionary<int, Dictionary<Vector2Int, FogStatus>>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        /// <summary>
        /// Called once after all floors are generated (but before the player spawns).
        /// Sets up a 100×100 coverage texture for each floor (skipping floor 0 if you like).
        /// </summary>
        public void InitializeAllFloors()
        {
            foreach (var kvp in DungeonManager.Instance.floors)
            {
                int floorNum = kvp.Key;
                if (floorNum == 0)
                    continue; // skip floor 0 if you want no fog in spawning room

                FloorData floorData = kvp.Value;
                SetupFogForFloor(floorNum, floorData);
            }
        }

        List<Vector2Int> allCells = new List<Vector2Int>();

        private void SetupFogForFloor(int floorNum, FloorData floorData)
        {
            allCells.Clear();
            allCells.AddRange(floorData.FloorTiles);
            allCells.AddRange(floorData.WallTiles);
            allCells.AddRange(floorData.VoidTiles);
            // 1) Make the FogStatus dictionary
            if (!floorFogStatus.ContainsKey(floorNum))
            {
                floorFogStatus[floorNum] = new Dictionary<Vector2Int, FogStatus>();
                foreach (Vector2Int cell in allCells)
                {
                    floorFogStatus[floorNum][cell] = FogStatus.Unexplored;
                }
            }

            // 2) Make a 100×100 coverage texture
            if (!fogTextures.ContainsKey(floorNum))
            {
                Texture2D tex = new Texture2D(100, 100, TextureFormat.RGBA32, false);
                tex.filterMode = FilterMode.Point;

                // Fill coverage=1 => black
                Color[] fill = new Color[100 * 100];
                for (int i = 0; i < fill.Length; i++)
                {
                    fill[i] = new Color(1f, 0f, 0f, 1f); // r=1 => full black overlay
                }
                tex.SetPixels(fill);
                tex.Apply();

                fogTextures[floorNum] = tex;
            }

            // 3) Create big sprite for overlay
            if (!overlayObjects.ContainsKey(floorNum))
            {
                GameObject go = new GameObject("FogOverlay_" + floorNum);
                if (floorData.FloorTilemap != null)
                    go.transform.SetParent(floorData.FloorTilemap.transform.parent);
                else
                    go.transform.SetParent(GameObject.Find("DungeonParent").transform);

                // Position so (0,0) lines up with floor, offset so it covers 0..100
                Vector3 floorPos = floorData.FloorTilemap.transform.position;
                Vector3 offset = new Vector3(50, 50, 0);
                go.transform.position = floorPos + offset;

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.material = fogAlphaMaterial;
                sr.sprite = oneByOneSprite;
                sr.sortingLayerName = "Fog";
                sr.sortingOrder = 4; // 4 is top for fog.

                // Scale 1×1 => 100×100
                go.transform.localScale = new Vector3(100, 100, 1);
                overlayObjects[floorNum] = go;
            }

            // 4) Assign the texture to material
            SpriteRenderer overlaySR = overlayObjects[floorNum].GetComponent<SpriteRenderer>();
            overlaySR.material.SetTexture("_FogMask", fogTextures[floorNum]);
        }

        /// <summary>
        /// Call this whenever the player moves on floorNum to reveal floor, wall, and void cells near the player.
        /// </summary>
        public void UpdateFog(Vector3 playerPos, FloorData floorData)
        {
            int floorNum = floorData.FloorNumber;
            if (!floorFogStatus.ContainsKey(floorNum))
                return;

            // Get the fog status dictionary for this floor.
            var statuses = floorFogStatus[floorNum];

            // Loop through ALL cells (floor, wall, and void) to update fog status.
            // (Note: Using ToList() to avoid modifying the collection during iteration.)
            foreach (Vector2Int cell in new List<Vector2Int>(statuses.Keys))
            {
                float dist = Vector2.Distance(playerPos, cell);
                if (dist <= visionRange)
                {
                    // If within vision range, mark as visible.
                    statuses[cell] = FogStatus.Visible;
                }
                else if (statuses[cell] == FogStatus.Visible)
                {
                    // If the cell was visible but is now out of range, mark as explored.
                    statuses[cell] = FogStatus.Explored;
                }
            }

            // Update the fog texture using the updated statuses.
            Texture2D tex = fogTextures[floorNum];
            Color[] pix = tex.GetPixels();

            foreach (KeyValuePair<Vector2Int, FogStatus> kvp in statuses)
            {
                Vector2Int cell = kvp.Key;
                int index = cell.x + cell.y * 100; // Assumes cells are within 0..99
                if (index < 0 || index >= pix.Length)
                    continue;

                float coverage;
                switch (kvp.Value)
                {
                    case FogStatus.Visible:
                        coverage = 0f; // Fully clear
                        break;
                    case FogStatus.Explored:
                        coverage = 0.85f; // Partially dark (gray)
                        break;
                    default: // FogStatus.Unexplored
                        coverage = 1f; // Fully black
                        break;
                }
                pix[index] = new Color(coverage, 0f, 0f, 1f);
            }

            tex.SetPixels(pix);
            tex.Apply();
        }

        public float GetFogCoverage(Vector2Int cell, int floorNum = 1)
        {
            if (
                !floorFogStatus.ContainsKey(floorNum) || !floorFogStatus[floorNum].ContainsKey(cell)
            )
                return 1f; // Default to fully fogged if cell not found.

            FogStatus status = floorFogStatus[floorNum][cell];
            switch (status)
            {
                case FogStatus.Visible:
                    return 0f;
                case FogStatus.Explored:
                    return 0.85f;
                default: // Unexplored
                    return 1f;
            }
        }

        public Texture2D GetFogTextureForFloor(int floorNum)
        {
            return fogTextures.ContainsKey(floorNum) ? fogTextures[floorNum] : null;
        }
    }
}
