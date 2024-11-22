using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace YourGameNamespace
{
    /// <summary>
    /// A self-contained debugging script that validates components and values on a GameObject.
    /// </summary>
    public class DebugValidator : MonoBehaviour
    {
        [Header("Validation Settings")]
        [Tooltip("Enable to validate automatically on Start.")]
        [SerializeField] private bool autoValidateOnStart = true;

        [Tooltip("Enable to log detailed validation results.")]
        [SerializeField] private bool detailedLogs = true;

        private void Start()
        {
            if (autoValidateOnStart)
            {
                ValidateGameObject();
            }
        }

        /// <summary>
        /// Validates the GameObject by inspecting attached components and their fields.
        /// </summary>
        public void ValidateGameObject()
        {
            Debug.Log($"[DebugValidator] Validating GameObject '{gameObject.name}'...");

            // Validate components
            var components = GetComponents<Component>();
            foreach (var component in components)
            {
                ValidateComponent(component);
            }

            Debug.Log($"[DebugValidator] Validation complete for GameObject '{gameObject.name}'.");
        }

        /// <summary>
        /// Validates a single component by inspecting its public fields and properties.
        /// </summary>
        private void ValidateComponent(Component component)
        {
            if (component == null)
            {
                Debug.LogError($"[DebugValidator] Missing or null component detected on '{gameObject.name}'.");
                return;
            }

            Type componentType = component.GetType();
            Debug.Log($"[DebugValidator] Validating Component: {componentType.Name} on '{gameObject.name}'.");

            // Validate fields
            FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                ValidateField(component, field);
            }

            // Validate properties
            PropertyInfo[] properties = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                ValidateProperty(component, property);
            }
        }

        /// <summary>
        /// Validates a single field of a component.
        /// </summary>
        private void ValidateField(Component component, FieldInfo field)
        {
            object value = field.GetValue(component);
            if (value == null || (value is UnityEngine.Object unityObject && unityObject == null))
            {
                Debug.LogError($"[DebugValidator] Field '{field.Name}' in Component '{component.GetType().Name}' is not assigned on '{gameObject.name}'.");
            }
            else if (detailedLogs)
            {
                Debug.Log($"[DebugValidator] Field '{field.Name}' in Component '{component.GetType().Name}' is valid with value: {value}.");
            }
        }

        /// <summary>
        /// Validates a single property of a component.
        /// </summary>
        private void ValidateProperty(Component component, PropertyInfo property)
        {
            if (!property.CanRead || property.GetIndexParameters().Length > 0)
            {
                // Skip write-only or indexed properties
                return;
            }

            object value;
            try
            {
                value = property.GetValue(component);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[DebugValidator] Unable to validate property '{property.Name}' in Component '{component.GetType().Name}' due to error: {e.Message}");
                return;
            }

            if (value == null || (value is UnityEngine.Object unityObject && unityObject == null))
            {
                Debug.LogError($"[DebugValidator] Property '{property.Name}' in Component '{component.GetType().Name}' is not assigned on '{gameObject.name}'.");
            }
            else if (detailedLogs)
            {
                Debug.Log($"[DebugValidator] Property '{property.Name}' in Component '{component.GetType().Name}' is valid with value: {value}.");
            }
        }
    }
}
