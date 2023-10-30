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

            var nodeMap = CreatePrefabMap(rootPrefab, node);

            HashSet<Object> internalReferences = new();
            internalReferences.UnionWith(GetAllComponentsAsObjects(rootPrefab));
            internalReferences.UnionWith(CreateInternalReferenceSet(node.WindowData.SerializedObject));

            HashSet<ObjectNode> visited = new();
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

                    var parentNode = nodeMap.ContainsKey(currentGameObject)
                        ? nodeMap[currentGameObject]
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

        private Dictionary<GameObject, ObjectNode> CreatePrefabMap(GameObject root, ObjectNode parent)
        {
            var map = new Dictionary<GameObject, ObjectNode>();
            var assetPrefabReferences = new Dictionary<string, ObjectNode>();
            var rootAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root);
            // Add children
            Queue<Tuple<GameObject, ObjectNode>> gameObjects = new();
            for (int i = 0; i < root.transform.childCount; ++i)
                gameObjects.Enqueue(new Tuple<GameObject, ObjectNode>(
                    item1: root.transform.GetChild(i).gameObject,
                    item2: parent));

            ObjectNode AddToMap(GameObject gameObject, ObjectNode currentParent)
            {
                // Check if it's a prefab instance
                var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
                if (string.IsNullOrEmpty(assetPath)) return currentParent;
                if (string.Equals(assetPath, rootAssetPath)) return currentParent;

                // Check if is a modified Prefab Instance
                var rootPrefabInstance = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);
                if (IsModifiedPrefabInstance(gameObject))
                {
                    if (rootPrefabInstance != null && map.ContainsKey(rootPrefabInstance))
                    {
                        map.Add(gameObject, map[rootPrefabInstance]);
                        return map[rootPrefabInstance];
                    }

                    var node = NodeFactory.CreateNode(gameObject, true);
                    map.Add(gameObject, node);
                    ObjectNode.CreateReference(
                        sourceObject: currentParent,
                        targetObject: node,
                        referenceType: ReferenceType.NestedPrefab);
                    return node;
                }

                // Create node to original Prefab
                if (!assetPrefabReferences.ContainsKey(assetPath))
                {
                    var originalAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    var assetNode = NodeFactory.CreateNode(originalAsset);
                    assetPrefabReferences.Add(assetPath, assetNode);
                    ObjectNode.CreateReference(
                        sourceObject: currentParent,
                        targetObject: assetNode,
                        referenceType: ReferenceType.NestedPrefab);
                    return assetNode;
                }

                // Map Object to original prefab
                var existingAssetNode = assetPrefabReferences[assetPath];
                map.Add(gameObject, existingAssetNode);
                ObjectNode.CreateReference(
                    sourceObject: currentParent,
                    targetObject: existingAssetNode,
                    referenceType: ReferenceType.NestedPrefab);
                return existingAssetNode;
            }

            while (gameObjects.Count > 0)
            {
                var dataPair = gameObjects.Dequeue();
                var gameObject = dataPair.Item1;
                var currentParent = dataPair.Item2;

                var newParent = AddToMap(gameObject, currentParent);

                // Enqueue children
                for (int i = 0; i < gameObject.transform.childCount; ++i)
                    gameObjects.Enqueue(new Tuple<GameObject, ObjectNode>(
                        item1: gameObject.transform.GetChild(i).gameObject,
                        item2: newParent));
            }

            return map;
        }

        private bool IsModifiedPrefabInstance(GameObject prefabInstance)
        {
            var addedComponents = PrefabUtility.GetAddedComponents(prefabInstance);
            if (addedComponents.Count > 0)
            {
                for (int i = 0; i < addedComponents.Count; ++i)
                    if (addedComponents[i].instanceComponent.gameObject == prefabInstance)
                        return true;
            }

            var removedComponents = PrefabUtility.GetRemovedComponents(prefabInstance);
            if (removedComponents.Count > 0)
            {
                for (int i = 0; i < removedComponents.Count; ++i)
                    if (removedComponents[i].containingInstanceGameObject == prefabInstance)
                        return true;
            }

            var modifications = PrefabUtility.GetPropertyModifications(prefabInstance);
            for (int i = 0; i < modifications.Length; ++i)
            {
                var mod = modifications[i];
                if (mod.target is Transform) continue;
                if (mod.target is GameObject) continue;
                return true;
            }

            return false;
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
