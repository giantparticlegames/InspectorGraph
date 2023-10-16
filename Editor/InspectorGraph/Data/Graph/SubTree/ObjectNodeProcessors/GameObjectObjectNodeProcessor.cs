// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using GiantParticle.InspectorGraph.Data.Nodes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Data.Graph.SubTree.ObjectNodeProcessors
{
    internal class GameObjectObjectNodeProcessor : BaseObjectNodeProcessor
    {
        public override Type TargetType => typeof(GameObject);

        public override void ProcessNode(ObjectNode node)
        {
            if (!(node.Object is GameObject rootPrefab)) return;

            Dictionary<string, ObjectNode> hierarchyMap = CreatePrefabHierarchyMap(rootPrefab, node);
            HashSet<Object> internalReferences = new();
            internalReferences.UnionWith(GetAllComponentsAsObjects(rootPrefab));
            internalReferences.UnionWith(CreateInternalReferenceSet(node.WindowData.SerializedObject));

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
                    string componentAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(currentComponent);

                    var parentNode = hierarchyMap.ContainsKey(componentAssetPath)
                        ? hierarchyMap[componentAssetPath]
                        : node;
                    var serializedComponent = new SerializedObject(currentComponent);
                    ProcessAllVisibleSerializedProperties(
                        parentNode: parentNode,
                        serializedObject: serializedComponent,
                        excludeReferences: internalReferences);
                }

                // Check child GameObjects
                for (int i = 0; i < currentGameObject.transform.childCount; ++i)
                    gameObjectQueue.Enqueue(currentGameObject.transform.GetChild(i).gameObject);
            }
        }

        private Dictionary<string, ObjectNode> CreatePrefabHierarchyMap(GameObject prefab, ObjectNode rootNode)
        {
            Dictionary<string, ObjectNode> map = new();
            Dictionary<string, HashSet<GameObject>> rootMap = new();

            void AddToMaps(GameObject obj, ObjectNode parentNode, bool isRoot = false)
            {
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                if (string.IsNullOrEmpty(assetPath)) return;

                var rootPrefab = PrefabUtility.GetNearestPrefabInstanceRoot(obj);
                if (isRoot)
                {
                    map.Add(assetPath, parentNode);
                    rootMap.Add(assetPath, new HashSet<GameObject> { rootPrefab });
                    return;
                }

                if (!map.ContainsKey(assetPath))
                {
                    ObjectNode node = NodeFactory.CreateNode(obj);
                    ObjectNode.CreateReference(
                        sourceObject: parentNode,
                        targetObject: node,
                        referenceType: ReferenceType.NestedPrefab);

                    // Add to maps
                    map.Add(assetPath, node);
                    rootMap.Add(assetPath, new HashSet<GameObject> { rootPrefab });
                    return;
                }

                // New duplicate nested prefab
                if (!rootMap.ContainsKey(assetPath)) return;
                if (rootMap[assetPath].Contains(rootPrefab)) return;

                // Add new reference
                rootMap[assetPath].Add(rootPrefab);
                ObjectNode newNode = NodeFactory.CreateNode(obj);
                ObjectNode.CreateReference(
                    sourceObject: parentNode,
                    targetObject: newNode,
                    referenceType: ReferenceType.NestedPrefab);
            }

            AddToMaps(prefab, rootNode, true);

            Queue<Tuple<GameObject, ObjectNode>> gameObjectQueue = new();
            gameObjectQueue.Enqueue(new Tuple<GameObject, ObjectNode>(prefab, rootNode));
            while (gameObjectQueue.Count > 0)
            {
                var pair = gameObjectQueue.Dequeue();
                GameObject currentGameObject = pair.Item1;
                ObjectNode parentNode = pair.Item2;

                AddToMaps(currentGameObject, parentNode);

                // Queue children
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(currentGameObject);
                for (int i = 0; i < currentGameObject.transform.childCount; ++i)
                {
                    GameObject child = currentGameObject.transform.GetChild(i).gameObject;
                    gameObjectQueue.Enqueue(
                        item: new Tuple<GameObject, ObjectNode>(
                            item1: child,
                            item2: map.ContainsKey(assetPath)
                                ? map[assetPath]
                                : rootNode));
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
