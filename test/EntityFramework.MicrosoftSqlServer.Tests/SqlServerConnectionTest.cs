// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.SqlClient;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage.Internal;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.Data.Entity.SqlServer.Tests
{
    public class SqlServerConnectionTest
    {
        [Fact]
        public void Creates_SQL_Server_connection_string()
        {
            using (var connection = new SqlServerConnection(CreateOptions(), new Logger<SqlServerConnection>(new LoggerFactory())))
            {
                Assert.IsType<SqlConnection>(connection.DbConnection);
            }
        }

        [Fact]
        public void Can_create_master_connection()
        {
            using (var connection = new SqlServerConnection(CreateOptions(), new Logger<SqlServerConnection>(new LoggerFactory())))
            {
                using (var master = connection.CreateMasterConnection())
                {
                    Assert.Equal(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True", master.ConnectionString);
                    Assert.Equal(SqlServerConnection.DefaultMasterConnectionCommandTimeout, master.CommandTimeout);
                }
            }
        }

        [Fact]
        public void Master_connection_string_none_default_command_timeout()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=SqlServerConnectionTest;Trusted_Connection=True;")
                .CommandTimeout(55);

            using (var connection = new SqlServerConnection(optionsBuilder.Options, new Logger<SqlServerConnection>(new LoggerFactory())))
            {
                using (var master = connection.CreateMasterConnection())
                {
                    Assert.Equal(55, master.CommandTimeout);
                }
            }
        }

        public static IDbContextOptions CreateOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=SqlServerConnectionTest;Trusted_Connection=True;");

            return optionsBuilder.Options;
        }
    }
}
