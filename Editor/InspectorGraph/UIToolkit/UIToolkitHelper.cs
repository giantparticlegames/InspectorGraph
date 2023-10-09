// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GiantParticle.InspectorGraph.UIToolkit
{
    internal static class UIToolkitHelper
    {
        public static VisualTreeAsset LocateViewForType(object obj)
        {
            var objectType = obj.GetType();
            string[] potentialGUIDs = AssetDatabase.FindAssets($"t:Script {objectType.Name}");
            for (int i = 0; i < potentialGUIDs.Length; ++i)
            {
                var guid = potentialGUIDs[i];
                var scriptPath = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileNameWithoutExtension(scriptPath);

                // Check if we found the script file
                if (!string.Equals(fileName, objectType.Name)) continue;

                // Check if a UXML file exist next to it
                var dirInfo = Directory.GetParent(scriptPath);
                if (dirInfo == null) continue;
                var parentFolder = dirInfo.ToString();
                var expectedUXMLName = $"{fileName}.uxml";
                var expectedPath = Path.Combine(parentFolder, expectedUXMLName);
                if (!File.Exists(expectedPath)) continue;

                string projectPath = Directory.GetParent(Application.dataPath).ToString();
                string relativePath = Path.GetRelativePath(projectPath, expectedPath);

                // Load file
                return AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(relativePath);
            }

            Debug.LogError($"Could not locate UXML file for [{objectType.Name}]");
            return null;
        }

        public static void ResolveVisualElements(object obj, VisualElement root)
        {
            var objectType = obj.GetType();
            var fields = objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo field = fields[i];
                var attribute = field.GetCustomAttribute<VisualElementFieldAttribute>(true);
                if (attribute == null) continue;
                string targetID = !string.IsNullOrEmpty(attribute.ElementID)
                    ? attribute.ElementID
                    : field.Name;

                // Query root object
                var element = root.Q(targetID);
                if (element == null) continue;

                // Check type
                if (!field.FieldType.IsAssignableFrom(element.GetType())) continue;

                // Assign
                field.SetValue(obj, element);
            }
        }
    }
}
