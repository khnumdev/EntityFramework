// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Query.ExpressionVisitors.Internal
{
    public class RelationalNavigationRewritingExpressionVisitorFactory : INavigationRewritingExpressionVisitorFactory
    {
        public virtual NavigationRewritingExpressionVisitor Create(EntityQueryModelVisitor queryModelVisitor)
            => new RelationalNavigationRewritingExpressionVisitor(queryModelVisitor);
    }
}
