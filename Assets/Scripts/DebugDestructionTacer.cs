using System;
using UnityEngine;

namespace YourGameNamespace
{
    public class DebugDestructionTracer : MonoBehaviour
    {
        private void OnDestroy()
        {
            // Get the stack trace to find what caused the destruction of this object
            string stackTrace = Environment.StackTrace;

            // Log a detailed message including the object name, position, and stack trace
            Debug.Log($"[DebugDestructionTracer] '{gameObject.name}' is being destroyed.\n" +
                      $"Position: {transform.position}\n" +
                      $"Active in Hierarchy: {gameObject.activeInHierarchy}\n" +
                      $"Stack Trace:\n{stackTrace}");
                bool isSceneChange = UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded;
    Debug.Log($"[DebugDestructionTracer] '{gameObject.name}' is being destroyed. " +
              $"Scene Reload: {isSceneChange}\nPosition: {transform.position}\n" +
              $"Stack Trace:\n{stackTrace}");
        }
    }
}
