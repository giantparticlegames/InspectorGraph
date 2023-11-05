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
        private class GameObjectPair
        {
            public readonly GameObject GameObject;
            public readonly ObjectNode Node;

            public GameObjectPair(GameObject gameObject, ObjectNode node)
            {
                GameObject = gameObject;
                Node = node;
            }
        }

        public override Type TargetType => typeof(GameObject);

        public override void ProcessNode(ObjectNode node)
        {
            if (!(node.Object is GameObject rootGameObject)) return;

            var nodeMap = CreatePrefabMap(rootGameObject, node);

            HashSet<Object> internalReferences = new();
            internalReferences.UnionWith(GetAllComponentsAsObjects(rootGameObject));
            internalReferences.UnionWith(CreateInternalReferenceSet(node.WindowData.SerializedObject));

            Queue<GameObjectPair> gameObjectQueue = new();
            gameObjectQueue.Enqueue(new GameObjectPair(rootGameObject, node));
            while (gameObjectQueue.Count > 0)
            {
                var pair = gameObjectQueue.Dequeue();
                GameObject currentGameObject = pair.GameObject;
                ObjectNode parentNode = nodeMap.ContainsKey(currentGameObject) ? nodeMap[currentGameObject] : pair.Node;
                // TODO: Figure out how to map a GameObject instance to the original Prefab GameObject in a hierarchy
                // This will allow to treat unmodified Prefab Instances as the original Prefab as reference

                // Check all components
                var components = currentGameObject.GetComponents<Component>();
                for (int i = 0; i < components.Length; ++i)
                {
                    var currentComponent = components[i];

                    var serializedComponent = new SerializedObject(currentComponent);
                    ProcessAllVisibleSerializedProperties(
                        parentNode: parentNode,
                        serializedObject: serializedComponent,
                        excludeReferences: internalReferences);
                }

                // Check child GameObjects
                for (int i = 0; i < currentGameObject.transform.childCount; ++i)
                    gameObjectQueue.Enqueue(new GameObjectPair(
                        currentGameObject.transform.GetChild(i).gameObject,
                        parentNode));
            }
        }

        private Dictionary<GameObject, ObjectNode> CreatePrefabMap(GameObject root, ObjectNode parent)
        {
            var map = new Dictionary<GameObject, ObjectNode>();
            var assetPrefabReferences = new Dictionary<string, ObjectNode>();
            var rootAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root);
            // Add children
            Queue<GameObjectPair> gameObjects = new();
            for (int i = 0; i < root.transform.childCount; ++i)
                gameObjects.Enqueue(new GameObjectPair(
                    root.transform.GetChild(i).gameObject,
                    parent));

            ObjectNode AddToMap(GameObject gameObject, ObjectNode currentParent)
            {
                // Check if it's a prefab instance
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
                if (string.IsNullOrEmpty(assetPath)) return currentParent;
                if (string.Equals(assetPath, rootAssetPath)) return currentParent;

                // Check if is a modified Prefab Instance
                var rootPrefabInstance = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);
                if (IsModifiedPrefabInstance(rootPrefabInstance))
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
                    map.Add(gameObject, assetNode);
                    ObjectNode.CreateReference(
                        sourceObject: currentParent,
                        targetObject: assetNode,
                        referenceType: ReferenceType.NestedPrefab);
                    return assetNode;
                }

                // Map Object to original prefab
                var existingAssetNode = assetPrefabReferences[assetPath];
                map.Add(gameObject, existingAssetNode);
                // Avoid creating self reference
                if (currentParent.Object != existingAssetNode.Object)
                {
                    ObjectNode.CreateReference(
                        sourceObject: currentParent,
                        targetObject: existingAssetNode,
                        referenceType: ReferenceType.NestedPrefab);
                }

                return existingAssetNode;
            }

            while (gameObjects.Count > 0)
            {
                var dataPair = gameObjects.Dequeue();
                var gameObject = dataPair.GameObject;
                var currentParent = dataPair.Node;

                var newParent = AddToMap(gameObject, currentParent);

                // Enqueue children
                for (int i = 0; i < gameObject.transform.childCount; ++i)
                    gameObjects.Enqueue(new GameObjectPair(
                        gameObject.transform.GetChild(i).gameObject,
                        newParent));
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

            var overrides = PrefabUtility.GetObjectOverrides(prefabInstance);
            for (int i = 0; i < overrides.Count; ++i)
            {
                var objectOverride = overrides[i];
                if (!(objectOverride.instanceObject is Component component)) continue;
                // Check if the target is a children of this instance
                if (IsObjectInHierarchy(prefabInstance, component.gameObject)) return true;
            }

            var addedObjects = PrefabUtility.GetAddedGameObjects(prefabInstance);
            for (int i = 0; i < addedObjects.Count; ++i)
            {
                if (!IsObjectInHierarchy(prefabInstance, addedObjects[i].instanceGameObject)) continue;
                return true;
            }

            return false;
        }

        private bool IsObjectInHierarchy(GameObject root, GameObject target)
        {
            Queue<GameObject> queue = new();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == target) return true;
                // Enqueue children
                for (int i = 0; i < current.transform.childCount; ++i)
                    queue.Enqueue(current.transform.GetChild(i).gameObject);
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
