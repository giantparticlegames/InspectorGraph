// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GiantParticle.InspectorGraph.Editor.Common
{
    internal static class ReflectionHelper
    {
        public static Type[] GetAllInterfaceImplementations(Type interfaceType, bool includeAbstract = false)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"Given type is not an interface: {interfaceType.FullName}");

            List<Type> allTypes = new();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; ++i)
            {
                Type[] assemblyTypes = assemblies[i].GetTypes();
                for (int t = 0; t < assemblyTypes.Length; ++t)
                {
                    Type assemblyType = assemblyTypes[t];
                    if (!interfaceType.IsAssignableFrom(assemblyType))
                        continue;
                    if (assemblyType.IsAbstract && !includeAbstract)
                        continue;
                    allTypes.Add(assemblyType);
                }
            }

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
    }
}
