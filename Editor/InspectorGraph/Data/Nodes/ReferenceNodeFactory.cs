// ********************************
// (C) 2022 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Editor.Data.Nodes.Filters;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes
{
    public class ReferenceNodeFactory
    {
        private readonly Queue<ObjectNode> _queue = new();

        private readonly ITypeFilterHandler _typeFilter;

        public ReferenceNodeFactory(ITypeFilterHandler filterHandler)
        {
            _typeFilter = filterHandler;
        }

        public IObjectNode CreateGraphFromObject(Object rootObject)
        {
            _queue.Enqueue(new ObjectNode(new WindowData(rootObject)));

            Dictionary<Object, ObjectNode> visitedObjects = new();
            ObjectNode root = null;
            while (_queue.Count > 0)
            {
                var node = _queue.Dequeue();

                if (visitedObjects.ContainsKey(node.Target)) continue;

                if (root == null) root = node;
                visitedObjects.Add(node.Target, node);

                if (!_typeFilter.ShouldExpandObject(node.Target))
                    continue;

                // Process Object
                if (node.Target is GameObject) ProcessGameObject(node);
                else ProcessSerializedObject(node, node.WindowData.SerializedTarget);
            }

            return root;
        }

        private void ProcessSerializedObject(ObjectNode parentNode, SerializedObject serializedObject,
            HashSet<Object> internalReferences = null)
        {
            // Scan direct references
            var iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (iterator.propertyType != SerializedPropertyType.ObjectReference)
                    continue;

                var reference = iterator.objectReferenceValue;
                if (reference == null)
                    continue;
                if (!_typeFilter.ShouldShowObject(reference))
                    continue;
                if (internalReferences != null && internalReferences.Contains(reference))
                    continue;

                // Translate component reference to Prefab
                if (reference is Component)
                {
                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(reference);
                    if (!string.IsNullOrEmpty(prefabPath))
                        reference = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
                }

                ObjectNode childNode = new ObjectNode(new WindowData(reference));
                parentNode.AddNode(childNode, ReferenceType.Direct);

                // Expand if indicated
                if (!_typeFilter.ShouldExpandObject(reference))
                    continue;
                _queue.Enqueue(childNode);
            }
        }

        private void ProcessGameObject(ObjectNode rootNode)
        {
            if (!_typeFilter.ShouldShowObject(rootNode.Target)) return;
            if (!(rootNode.Target is GameObject rootPrefab)) return;

            var hierarchyMap = CreateHierarchyMap(rootPrefab, rootNode);
            var allInternalComponents = GetAllComponentsAsObjects(rootPrefab);

            Queue<GameObject> gameObjectQueue = new();
            gameObjectQueue.Enqueue(rootPrefab);
            while (gameObjectQueue.Count > 0)
            {
                GameObject currentGameObject = gameObjectQueue.Dequeue();

                // Check all components
                var components = currentGameObject.GetComponents<Component>();
                for (int i = 0; i < components.Length; ++i)
                {
                    var currentComponent = components[i];
                    string componentPrefabSource =
                        PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(currentComponent);

                    var parentNode = hierarchyMap[componentPrefabSource];
                    var serializedComponent = new SerializedObject(currentComponent);
                    ProcessSerializedObject(parentNode, serializedComponent, allInternalComponents);
                }

                // Check child GameObjects
                for (int i = 0; i < currentGameObject.transform.childCount; ++i)
                    gameObjectQueue.Enqueue(currentGameObject.transform.GetChild(i).gameObject);
            }
        }

        private Dictionary<string, ObjectNode> CreateHierarchyMap(GameObject prefab, ObjectNode rootNode)
        {
            Dictionary<string, ObjectNode> map = new();
            string rootPrefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
            map.Add(rootPrefabPath, rootNode);

            Queue<Tuple<GameObject, ObjectNode>> gameObjectQueue = new();
            gameObjectQueue.Enqueue(new Tuple<GameObject, ObjectNode>(prefab, rootNode));
            while (gameObjectQueue.Count > 0)
            {
                var pair = gameObjectQueue.Dequeue();
                GameObject currentGameObject = pair.Item1;
                ObjectNode parentNode = pair.Item2;

                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(currentGameObject);
                if (!map.ContainsKey(assetPath))
                {
                    ObjectNode node = new ObjectNode(new WindowData(currentGameObject));
                    parentNode.AddNode(node, ReferenceType.HierarchyEmbedded);
                    map.Add(assetPath, node);
                }

                // Queue children
                for (int i = 0; i < currentGameObject.transform.childCount; ++i)
                {
                    GameObject child = currentGameObject.transform.GetChild(i).gameObject;
                    gameObjectQueue.Enqueue(new Tuple<GameObject, ObjectNode>(child, map[assetPath]));
                }
            }

            return map;
        }

        private HashSet<Object> GetAllComponentsAsObjects(GameObject root)
        {
            HashSet<Object> components = new();
            components.UnionWith(root.GetComponents<Component>());
            components.UnionWith(root.GetComponentsInChildren<Component>(true));
            return components;
        }
    }
}
