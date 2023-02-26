// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;
using System.Collections.Generic;
using System.Reflection;

namespace GiantParticle.InspectorGraph.Editor.Common
{
    public class ReflectionHelper
    {
        public static Type[] GetAllInterfaceImplementations(Type interfaceType, bool includeAbstract = false)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"Given type is not an interface: {interfaceType.FullName}");

            List<Type> allTypes = new();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] assemblyTypes = assembly.GetTypes();
                foreach (Type assemblyType in assemblyTypes)
                {
                    if (!interfaceType.IsAssignableFrom(assemblyType))
                        continue;
                    if (assemblyType.IsAbstract && !includeAbstract)
                        continue;
                    allTypes.Add(assemblyType);
                }
            }

            return allTypes.ToArray();
        }
    }
}
