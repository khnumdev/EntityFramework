// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Query.ExpressionVisitors.Internal
{
    public class RelationalNavigationRewritingExpressionVisitor : NavigationRewritingExpressionVisitor
    {
        public RelationalNavigationRewritingExpressionVisitor([NotNull] EntityQueryModelVisitor queryModelVisitor)
            : base(Check.NotNull(queryModelVisitor, nameof(queryModelVisitor)))
        {
        }

        private RelationalNavigationRewritingExpressionVisitor(
            EntityQueryModelVisitor queryModelVisitor, IAsyncQueryProvider entityQueryProvider, NavigationRewritingExpressionVisitor parentvisitor)
            : this(queryModelVisitor)
        {
            QueryProvider = entityQueryProvider;
            ParentVisitor = parentvisitor;
        }

        public override NavigationRewritingExpressionVisitor CreateVisitorForSubQuery()
            => new RelationalNavigationRewritingExpressionVisitor(QueryModelVisitor, QueryProvider, this);

        protected override Expression TryOptimizeNavigation(MemberExpression memberExpression, IList<INavigation> navigations)
        {
            Check.NotNull(memberExpression, nameof(memberExpression));
            Check.NotNull(navigations, nameof(navigations));

            if ((navigations.Count == 1)
                && navigations[0].IsDependentToPrincipal())
            {
                var navigation = navigations[0];
                var principalKey = navigation.ForeignKey.PrincipalKey;
                if (principalKey.Properties.Count == 1)
                {
                    var principalKeyProperty = principalKey.Properties[0];
                    if (principalKeyProperty.Name == memberExpression.Member.Name)
                    {
                        Debug.Assert(navigation.ForeignKey.Properties.Count == 1);

                        var declaringExpression = ((MemberExpression)memberExpression.Expression).Expression;
                        var foreignKeyPropertyExpression = CreateKeyAccessExpression(declaringExpression, navigation.ForeignKey.Properties);

                        return foreignKeyPropertyExpression.Type != principalKeyProperty.ClrType
                            ? Expression.Convert(foreignKeyPropertyExpression, principalKeyProperty.ClrType)
                            : foreignKeyPropertyExpression;
                    }
                }
            }

            return null;
        }
    }
}
