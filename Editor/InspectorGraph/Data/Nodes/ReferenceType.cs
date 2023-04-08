// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes
{
    internal interface IExtendedReferenceType
    {
        string ToStringRepresentation(int value);
    }

    [Serializable]
    internal struct ReferenceType
    {
        private const int kDirectValue = 1;
        public static readonly ReferenceType Direct = kDirectValue;
        private const int kNestedPrefabValue = 2;
        public static readonly ReferenceType NestedPrefab = kNestedPrefabValue;

        public int Value;

        public ReferenceType(int value)
        {
            Value = value;
        }

        public static implicit operator int(ReferenceType typeS) => typeS.Value;
        public static implicit operator ReferenceType(int v) => new ReferenceType(v);

        public override string ToString()
        {
            switch (Value)
            {
                case kDirectValue:
                    return "Direct Reference";
                case kNestedPrefabValue:
                    return "Nested Prefab Reference";
            }

            var instances = ReflectionHelper.InstantiateAllImplementations<IExtendedReferenceType>();
            for (int i = 0; i < instances.Length; ++i)
            {
                var result = instances[i].ToStringRepresentation(Value);
                if (string.IsNullOrEmpty(result)) continue;
                return result;
            }

            return "N/A";
        }
    }
}
