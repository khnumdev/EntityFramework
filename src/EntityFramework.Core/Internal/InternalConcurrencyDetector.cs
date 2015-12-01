// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Data.Entity.Internal
{
    public class InternalConcurrencyDetector : IInternalConcurrencyDetector
    {
        private long _isInCriticalSection;

        public virtual void EnterCriticalSection()
        {
            if(Interlocked.CompareExchange(ref _isInCriticalSection, 1, 0) != 0)
            {
                throw new NotSupportedException("");
            }
        }

        public virtual void ExitCriticalSection()
        {
            var state = Interlocked.Exchange(ref _isInCriticalSection, 0);
            Debug.Assert(state == 1, "Expected to be in a critical section");
        }

        public virtual void EnsureNotInCriticalSection()
        {
            if (Interlocked.Read(ref _isInCriticalSection) != 0)
            {
                throw new NotSupportedException("");
            }
        }
    }
}
