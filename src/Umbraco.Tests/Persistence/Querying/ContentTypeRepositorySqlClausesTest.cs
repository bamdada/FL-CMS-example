using System;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Models.Rdbms;
using Umbraco.Core.Persistence;
using Umbraco.Tests.TestHelpers;

namespace Umbraco.Tests.Persistence.Querying
{
    [TestFixture]
    public class ContentTypeRepositorySqlClausesTest : BaseUsingSqlCeSyntax
    {
        [Test]
        public void Can_Verify_Base_Clause()
        {
            var NodeObjectType = new Guid(Constants.ObjectTypes.DocumentType);

            var expected = new Sql();
            expected.Select("*")
                .From("[cmsDocumentType]")
                .RightJoin("[cmsContentType]")
                .On("[cmsContentType].[nodeId] = [cmsDocumentType].[contentTypeNodeId]")
                .InnerJoin("[umbracoNode]")
                .On("[cmsContentType].[nodeId] = [umbracoNode].[id]")
                .Where("[umbracoNode].[nodeObjectType] = 'a2cb7800-f571-4787-9638-bc48539a0efb'")
                .Where("[cmsDocumentType].[IsDefault] = 1");

            var sql = new Sql();
            sql.Select("*")
                .From<DocumentTypeDto>()
                .RightJoin<ContentTypeDto>()
                .On<ContentTypeDto, DocumentTypeDto>(left => left.NodeId, right => right.ContentTypeNodeId)
                .InnerJoin<NodeDto>()
                .On<ContentTypeDto, NodeDto>(left => left.NodeId, right => right.NodeId)
                .Where<NodeDto>(x => x.NodeObjectType == NodeObjectType)
                .Where<DocumentTypeDto>(x => x.IsDefault == true);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));

            Console.WriteLine(sql.SQL);
        }

        [Test]
        public void Can_Verify_Base_Where_Clause()
        {
            var NodeObjectType = new Guid(Constants.ObjectTypes.DocumentType);

            var expected = new Sql();
            expected.Select("*")
                .From("[cmsDocumentType]")
                .RightJoin("[cmsContentType]")
                .On("[cmsContentType].[nodeId] = [cmsDocumentType].[contentTypeNodeId]")
                .InnerJoin("[umbracoNode]")
                .On("[cmsContentType].[nodeId] = [umbracoNode].[id]")
                .Where("[umbracoNode].[nodeObjectType] = 'a2cb7800-f571-4787-9638-bc48539a0efb'")
                .Where("[cmsDocumentType].[IsDefault] = 1")
                .Where("[umbracoNode].[id] = 1050");

            var sql = new Sql();
            sql.Select("*")
                .From<DocumentTypeDto>()
                .RightJoin<ContentTypeDto>()
                .On<ContentTypeDto, DocumentTypeDto>(left => left.NodeId, right => right.ContentTypeNodeId)
                .InnerJoin<NodeDto>()
                .On<ContentTypeDto, NodeDto>(left => left.NodeId, right => right.NodeId)
                .Where<NodeDto>(x => x.NodeObjectType == NodeObjectType)
                .Where<DocumentTypeDto>(x => x.IsDefault == true)
                .Where<NodeDto>(x => x.NodeId == 1050);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));

            Console.WriteLine(sql.SQL);
        }

        [Test]
        public void Can_Verify_PerformQuery_Clause()
        {
            var expected = new Sql();
            expected.Select("*")
                .From("[cmsPropertyTypeGroup]")
                .RightJoin("[cmsPropertyType]").On("[cmsPropertyTypeGroup].[id] = [cmsPropertyType].[propertyTypeGroupId]")
                .InnerJoin("[cmsDataType]").On("[cmsPropertyType].[dataTypeId] = [cmsDataType].[nodeId]");

            var sql = new Sql();
            sql.Select("*")
               .From<PropertyTypeGroupDto>()
               .RightJoin<PropertyTypeDto>()
               .On<PropertyTypeGroupDto, PropertyTypeDto>(left => left.Id, right => right.PropertyTypeGroupId)
               .InnerJoin<DataTypeDto>()
               .On<PropertyTypeDto, DataTypeDto>(left => left.DataTypeId, right => right.DataTypeId);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));

            Console.WriteLine(sql.SQL);
        }

        [Test]
        public void Can_Verify_AllowedContentTypeIds_Clause()
        {
            var expected = new Sql();
            expected.Select("*")
                .From("[cmsContentTypeAllowedContentType]")
                .Where("[cmsContentTypeAllowedContentType].[Id] = 1050");

            var sql = new Sql();
            sql.Select("*")
               .From<ContentTypeAllowedContentTypeDto>()
               .Where<ContentTypeAllowedContentTypeDto>(x => x.Id == 1050);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));

            Console.WriteLine(sql.SQL);
        }

        [Test]
        public void Can_Verify_PropertyGroupCollection_Clause()
        {
            var expected = new Sql();
            expected.Select("*")
                .From("[cmsPropertyTypeGroup]")
                .RightJoin("[cmsPropertyType]").On("[cmsPropertyTypeGroup].[id] = [cmsPropertyType].[propertyTypeGroupId]")
                .InnerJoin("[cmsDataType]").On("[cmsPropertyType].[dataTypeId] = [cmsDataType].[nodeId]")
                .Where("[cmsPropertyType].[contentTypeId] = 1050");

            var sql = new Sql();
            sql.Select("*")
               .From<PropertyTypeGroupDto>()
               .RightJoin<PropertyTypeDto>()
               .On<PropertyTypeGroupDto, PropertyTypeDto>(left => left.Id, right => right.PropertyTypeGroupId)
               .InnerJoin<DataTypeDto>()
               .On<PropertyTypeDto, DataTypeDto>(left => left.DataTypeId, right => right.DataTypeId)
               .Where<PropertyTypeDto>(x => x.ContentTypeId == 1050);

            Assert.That(sql.SQL, Is.EqualTo(expected.SQL));

            Console.WriteLine(sql.SQL);
        }
    }
}