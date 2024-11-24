using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoED
{
    public static class VectorConversion
    {
        /// <summary>
        /// Converts a HashSet of Vector3Int to a HashSet of Vector2Int by dropping the z component.
        /// </summary>
        public static HashSet<Vector2Int> Vector3SetToVector2Set(HashSet<Vector3Int> vector3Set)
        {
            if (vector3Set == null)
            {
                Debug.LogError("VectorConversion: Input HashSet<Vector3Int> is null.");
                return new HashSet<Vector2Int>();
            }

            return new HashSet<Vector2Int>(vector3Set.Select(v3 => new Vector2Int(v3.x, v3.y)));
        }

        /// <summary>
        /// Converts a HashSet of Vector2Int to a HashSet of Vector3Int by adding a z component of 0.
        /// </summary>
        public static HashSet<Vector3Int> Vector2SetToVector3Set(HashSet<Vector2Int> vector2Set)
        {
            if (vector2Set == null)
            {
                Debug.LogError("VectorConversion: Input HashSet<Vector2Int> is null.");
                return new HashSet<Vector3Int>();
            }

            return new HashSet<Vector3Int>(vector2Set.Select(v2 => new Vector3Int(v2.x, v2.y, 0)));
        }

        /// <summary>
        /// Converts a List of Vector3 to a List of Vector2Int by rounding the x and y components.
        /// </summary>
        public static List<Vector2Int> Vector3ListToVector2IntList(List<Vector3> vector3List)
        {
            if (vector3List == null)
            {
                Debug.LogError("VectorConversion: Input List<Vector3> is null.");
                return new List<Vector2Int>();
            }

            return vector3List.Select(p => new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y))).ToList();
        }

        /// <summary>
        /// Converts a List of Vector2Int to a List of Vector3Int by adding a z component of 0.
        /// </summary>
        public static List<Vector3Int> Vector2IntListToVector3IntList(List<Vector2Int> vector2List)
        {
            if (vector2List == null)
            {
                Debug.LogError("VectorConversion: Input List<Vector2Int> is null.");
                return new List<Vector3Int>();
            }

            return vector2List.Select(v2 => new Vector3Int(v2.x, v2.y, 0)).ToList();
        }
        /// <summary>
        /// Converts a HashSet of Vector2Int to a List of Vector3 by adding a z component of 0.
        /// </summary>
        public static List<Vector3> Vector2SetToVector3List(HashSet<Vector2Int> vector2Set)
        {
            if (vector2Set == null)
            {
                Debug.LogError("VectorConversion: Input HashSet<Vector2Int> is null.");
                return new List<Vector3>();
            }

            return vector2Set.Select(v2 => new Vector3(v2.x, v2.y, 0)).ToList();
        }

    }
}
