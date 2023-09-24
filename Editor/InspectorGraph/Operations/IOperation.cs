// ********************************
// (C) 2023 - Giant Particle Games
// All rights reserved.
// ********************************

using System;

namespace GiantParticle.InspectorGraph.Operations
{
    internal interface IOperation
    {
        OperationState State { get; }
        float Progress { get; }
        string Message { get; }
    }

    internal interface IOperation<T> : IOperation
    {
        T Result { get; }
    }
}
