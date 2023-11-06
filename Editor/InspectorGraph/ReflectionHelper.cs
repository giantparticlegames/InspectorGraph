// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using System.Reflection;
using GiantParticle.InspectorGraph.CustomAttributes;
using UnityEngine;

namespace GiantParticle.InspectorGraph
{
    internal static class ReflectionHelper
    {
        public static TInterface[] InstantiateAllImplementations<TInterface>()
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"Given type is not an interface: {interfaceType.FullName}");

            List<Type> allTypes = new();

            ProcessAssemblyTypes(
                assemblies: AppDomain.CurrentDomain.GetAssemblies(),
                action: type =>
                {
                    if (!interfaceType.IsAssignableFrom(type))
                        return;
                    if (type.IsAbstract || type.IsInterface)
                        return;
                    allTypes.Add(type);
                });

            List<TInterface> instances = new List<TInterface>(allTypes.Count);
            for (int i = 0; i < allTypes.Count; ++i)
            {
                var instance = (TInterface)Activator.CreateInstance(allTypes[i]);
                instances.Add(instance);
            }

            return instances.ToArray();
        }

        public static Type[] GetAllInheritors(Type baseType, bool includeAbstract = false,
            bool includeInterface = false)
        {
            List<Type> allTypes = new();
            ProcessAssemblyTypes(
                assemblies: AppDomain.CurrentDomain.GetAssemblies(),
                action: type =>
                {
                    if (!baseType.IsAssignableFrom(type))
                        return;
                    if (type.IsInterface && !includeInterface)
                        return;
                    if (type.IsAbstract && !includeAbstract)
                        return;
                    allTypes.Add(type);
                });

            return allTypes.ToArray();
        }

        public static Type GetTypeByName(string fullyQualifiedName)
        {
            Type type = Type.GetType(fullyQualifiedName);
            if (type != null) return type;

            // Search in all assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; ++i)
            {
                Type[] assemblyTypes = assemblies[i].GetTypes();
                for (int t = 0; t < assemblyTypes.Length; ++t)
                {
                    Type assemblyType = assemblyTypes[t];
                    if (string.Equals(assemblyType.FullName, fullyQualifiedName))
                        return assemblyType;
                }
            }

            Debug.LogWarning($"Type does not exist for [{fullyQualifiedName}]");
            return null;
        }

        public static List<Type> ListAllTypes()
        {
            List<Type> allTypes = new();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; ++i)
                allTypes.AddRange(assemblies[i].GetTypes());

            return allTypes;
        }

        private static void ProcessAssemblyTypes(Assembly[] assemblies, Action<Type> action)
        {
            for (int a = 0; a < assemblies.Length; ++a)
            {
                var types = assemblies[a].GetTypes();
                for (int t = 0; t < types.Length; ++t)
                {
                    action.Invoke(types[t]);
                }
            }
        }

        public static int CompareByPriority<T>(T objA, T objB)
        {
            Type priorityType = typeof(InternalPriorityAttribute);
            var priorityAttributeA = (InternalPriorityAttribute)Attribute.GetCustomAttribute(
                element: objA.GetType(),
                attributeType: priorityType);
            int priorityA = priorityAttributeA?.Priority ?? int.MaxValue;

            var priorityAttributeB = (InternalPriorityAttribute)Attribute.GetCustomAttribute(
                element: objB.GetType(),
                attributeType: priorityType);
            int priorityB = priorityAttributeB?.Priority ?? int.MaxValue;

            return priorityA - priorityB;
        }
    }
}
