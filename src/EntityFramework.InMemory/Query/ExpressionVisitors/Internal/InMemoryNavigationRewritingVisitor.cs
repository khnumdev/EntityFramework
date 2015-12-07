// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Query.ExpressionVisitors.Internal
{
    public class InMemoryNavigationRewritingExpressionVisitor : NavigationRewritingExpressionVisitor
    {
        public InMemoryNavigationRewritingExpressionVisitor([NotNull] EntityQueryModelVisitor queryModelVisitor)
            : base(Check.NotNull(queryModelVisitor, nameof(queryModelVisitor)))
        {
        }
    }
}
