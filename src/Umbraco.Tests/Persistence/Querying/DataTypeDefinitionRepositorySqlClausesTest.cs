using System;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Models.Rdbms;
using Umbraco.Core.Persistence;
using Umbraco.Tests.TestHelpers;

namespace Umbraco.Tests.Persistence.Querying
{
    [TestFixture]
    public class DataTypeDefinitionRepositorySqlClausesTest : BaseUsingSqlCeSyntax
    {
        [Test]
        public void Can_Verify_Base_Clause()
        {
            var NodeObjectTypeId = new Guid(Constants.ObjectTypes.DataType);

            var expected = new Sql();
            expected.Select("*")
                .From("[cmsDataType]")
                .InnerJoin("[umbracoNode]").On("[cmsDataType].[nodeId] = [umbracoNode].[id]")
                .Where("[umbracoNode].[nodeObjectType] = '30a2a501-1978-4ddb-a57b-f7efed43ba3c'");

            var sql = new Sql();
            sql.Select("*")
               .From<DataTypeDto>()
               .InnerJoin<NodeDto>()
               .On<DataTypeDto, NodeDto>(left => left.DataTypeId, right => right.NodeId)
               .Where<NodeDto>(x => x.NodeObjectType == NodeObjectTypeId);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));

            Console.WriteLine(sql.SQL);
        }
    }
}