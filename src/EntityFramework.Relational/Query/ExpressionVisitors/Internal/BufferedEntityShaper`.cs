﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Storage;
using Remotion.Linq.Clauses;

namespace Microsoft.Data.Entity.Query.ExpressionVisitors.Internal
{
    public class BufferedEntityShaper<TEntity> : EntityShaper, IShaper<TEntity>
        where TEntity : class
    {
        public BufferedEntityShaper(
            [NotNull] IQuerySource querySource,
            [NotNull] string entityType,
            bool trackingQuery,
            [NotNull] KeyValueFactory keyValueFactory,
            [NotNull] Func<ValueBuffer, object> materializer)
            : base(querySource, entityType, trackingQuery, keyValueFactory, materializer)
        {
        }

        public override Type Type => typeof(TEntity);

        public virtual TEntity Shape(QueryContext queryContext, ValueBuffer valueBuffer)
        {
            Debug.Assert(queryContext != null);

            var keyValue = KeyValueFactory.Create(valueBuffer);

            if (keyValue.IsInvalid)
            {
                if (!AllowNullResult)
                {
                    throw new InvalidOperationException(
                        RelationalStrings.InvalidKeyValue(EntityType));
                }
            }
            else
            {
                return (TEntity)queryContext.QueryBuffer
                    .GetEntity(
                        keyValue,
                        new EntityLoadInfo(valueBuffer, Materializer),
                        queryStateManager: IsTrackingQuery);
            }

            return null;
        }

        public override EntityShaper WithOffset(int offset)
            => new BufferedOffsetEntityShaper<TEntity>(
                QuerySource,
                EntityType,
                IsTrackingQuery,
                KeyValueFactory,
                Materializer)
                .SetOffset(offset);
    }
}
