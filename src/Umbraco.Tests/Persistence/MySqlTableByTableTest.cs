using System;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.ObjectResolution;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Tests.TestHelpers;

namespace Umbraco.Tests.Persistence
{
    [TestFixture, NUnit.Framework.Ignore]
    public class MySqlTableByTableTest : BaseTableByTableTest
    {
        private Database _database;

        #region Overrides of BaseTableByTableTest

        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            SqlSyntaxContext.SqlSyntaxProvider = MySqlSyntax.Provider;

            _database = new Database("Server = 169.254.120.3; Database = testdb; Uid = umbraco; Pwd = umbraco",
                                     "MySql.Data.MySqlClient");
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        public override Database Database
        {
            get { return _database; }
        }

        #endregion
    }
}