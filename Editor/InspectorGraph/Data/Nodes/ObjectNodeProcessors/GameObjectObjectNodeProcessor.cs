// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes.ObjectNodeProcessors
{
    internal class GameObjectObjectNodeProcessor : BaseObjectNodeProcessor
    {
        public override Type TargetType => typeof(GameObject);

        public override void ProcessNode(ObjectNode node)
        {
            if (!(node.Target is GameObject rootPrefab)) return;

            Dictionary<string, ObjectNode> hierarchyMap = CreateHierarchyMap(rootPrefab, node);
            HashSet<Object> internalReferences = new();
            internalReferences.UnionWith(GetAllComponentsAsObjects(rootPrefab));
            internalReferences.UnionWith(CreateInternalReferenceSet(node.WindowData.SerializedTarget));

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

                    var parentNode = hierarchyMap[componentAssetPath];
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
                    parentNode.AddNode(node, ReferenceType.NestedPrefab);
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
