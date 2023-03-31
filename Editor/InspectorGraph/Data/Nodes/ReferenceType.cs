// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Editor.Data.Nodes
{
    [Serializable]
    internal partial struct ReferenceType
    {
        public const int kDirectValue = 1;
        public static readonly ReferenceType Direct = kDirectValue;
        public const int kNestedPrefabValue = 2;
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

            string retValue = "N/A";
            ToStringExtended(ref retValue);
            return retValue;
        }

        partial void ToStringExtended(ref string text);
    }
}
