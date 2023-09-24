// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Operations
{
    internal class Operation : IOperation
    {
        public OperationState State { get; set; }

        public float Progress { get; set; }

        public string Message { get; set; }
    }

    internal class Operation<T> : Operation, IOperation<T>
    {
        public T Result
        {
            get;
            set;
        }
    }
}
