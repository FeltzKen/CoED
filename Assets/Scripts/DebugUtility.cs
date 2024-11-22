using UnityEngine;

namespace YourGameNamespace
{
    public static class DebugUtility
    {
        /// <summary>
        /// Validates if a component exists on a GameObject.
        /// </summary>
        public static bool ValidateComponent<T>(GameObject obj, string message = null) where T : Component
        {
            if (obj == null)
            {
                Debug.LogError($"Validation failed: GameObject is null. {message}");
                return false;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Validation failed: {typeof(T).Name} not found on GameObject '{obj.name}'. {message}");
                return false;
            }

            Debug.Log($"Validation successful: {typeof(T).Name} found on GameObject '{obj.name}'. {message}");
            return true;
        }

        /// <summary>
        /// Validates if a value is not null.
        /// </summary>
        public static bool ValidateNotNull(object value, string valueName, string message = null)
        {
            if (value == null)
            {
                Debug.LogError($"Validation failed: {valueName} is null. {message}");
                return false;
            }

            Debug.Log($"Validation successful: {valueName} is not null. {message}");
            return true;
        }

        /// <summary>
        /// Validates if a collection is not empty.
        /// </summary>
        public static bool ValidateCollection<T>(System.Collections.Generic.IEnumerable<T> collection, string collectionName, string message = null)
        {
            if (collection == null)
            {
                Debug.LogError($"Validation failed: {collectionName} is null. {message}");
                return false;
            }

            int count = 0;
            foreach (var _ in collection) count++;

            if (count == 0)
            {
                Debug.LogError($"Validation failed: {collectionName} is empty. {message}");
                return false;
            }

            Debug.Log($"Validation successful: {collectionName} has {count} items. {message}");
            return true;
        }

        /// <summary>
        /// Logs the properties of a FloorData object for debugging.
        /// </summary>
        public static void LogFloorData(FloorData floor)
        {
            if (!ValidateNotNull(floor, nameof(floor)))
                return;

            Debug.Log($"Validating Floor {floor.FloorNumber}...");
            Debug.Log($"Floor {floor.FloorNumber}: {floor.FloorTiles.Count} floor tiles.");
            Debug.Log($"Floor {floor.FloorNumber}: {floor.Rooms.Count} rooms.");
            Debug.Log($"Floor {floor.FloorNumber}: {floor.Corridors.Count} corridors.");
            Debug.Log($"Floor {floor.FloorNumber}: {floor.Connections.Count} room connections.");
            Debug.Log($"Floor {floor.FloorNumber}: {floor.PatrolPoints.Count} patrol points.");
        }

        /// <summary>
        /// Logs a message with context.
        /// </summary>
        public static void LogWithContext(string message, Object context = null)
        {
            if (context != null)
            {
                Debug.Log(message, context);
            }
            else
            {
                Debug.Log(message);
            }
        }
    }
}
