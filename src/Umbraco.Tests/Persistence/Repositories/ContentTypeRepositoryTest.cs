using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Repositories;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Tests.TestHelpers;
using Umbraco.Tests.TestHelpers.Entities;

namespace Umbraco.Tests.Persistence.Repositories
{
    [TestFixture]
    public class ContentTypeRepositoryTest : BaseDatabaseFactoryTest
    {
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            CreateTestData();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        //TODO Add test to verify SetDefaultTemplates updates both AllowedTemplates and DefaultTemplate(id).

        [Test]
        public void Can_Instantiate_Repository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();

            // Act
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            // Assert
            Assert.That(repository, Is.Not.Null);
        }

        [Test]
        public void Can_Perform_Add_On_ContentTypeRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            // Act
            var contentType = MockedContentTypes.CreateSimpleContentType();
            repository.AddOrUpdate(contentType);
            unitOfWork.Commit();

            // Assert
            Assert.That(contentType.HasIdentity, Is.True);
            Assert.That(contentType.PropertyGroups.All(x => x.HasIdentity), Is.True);
            Assert.That(contentType.Path.Contains(","), Is.True);
            Assert.That(contentType.SortOrder, Is.GreaterThan(0));
        }

        [Test]
        public void Can_Perform_Update_On_ContentTypeRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            // Act
            var contentType = repository.Get(1046);

            contentType.Thumbnail = "Doc2.png";
            contentType.PropertyGroups["Content"].PropertyTypes.Add(new PropertyType(new Guid(), DataTypeDatabaseType.Ntext)
            {
                Alias = "subtitle",
                Name = "Subtitle",
                Description = "Optional Subtitle",
                HelpText = "",
                Mandatory = false,
                SortOrder = 1,
                DataTypeDefinitionId = -88
            });
            repository.AddOrUpdate(contentType);
            unitOfWork.Commit();

            var dirty = ((ICanBeDirty) contentType).IsDirty();

            // Assert
            Assert.That(contentType.HasIdentity, Is.True);
            Assert.That(dirty, Is.False);
            Assert.That(contentType.Thumbnail, Is.EqualTo("Doc2.png"));
            Assert.That(contentType.PropertyTypes.Any(x => x.Alias == "subtitle"), Is.True);
        }

        [Test]
        public void Can_Perform_Delete_On_ContentTypeRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            // Act
            var contentType = MockedContentTypes.CreateSimpleContentType();
            repository.AddOrUpdate(contentType);
            unitOfWork.Commit();

            var contentType2 = repository.Get(contentType.Id);
            repository.Delete(contentType2);
            unitOfWork.Commit();

            var exists = repository.Exists(contentType.Id);

            // Assert
            Assert.That(exists, Is.False);
        }

        [Test]
        public void Can_Perform_Get_On_ContentTypeRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            // Act
            var contentType = repository.Get(1046);

            // Assert
            Assert.That(contentType, Is.Not.Null);
            Assert.That(contentType.Id, Is.EqualTo(1046));
        }

        [Test]
        public void Can_Perform_GetAll_On_ContentTypeRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            // Act
            var contentTypes = repository.GetAll();
            int count =
                DatabaseContext.Database.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM umbracoNode WHERE nodeObjectType = @NodeObjectType",
                    new { NodeObjectType = new Guid(Constants.ObjectTypes.DocumentType) });

            // Assert
            Assert.That(contentTypes.Any(), Is.True);
            Assert.That(contentTypes.Count(), Is.EqualTo(count));
        }

        [Test]
        public void Can_Perform_Exists_On_ContentTypeRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            // Act
            var exists = repository.Exists(1045);

            // Assert
            Assert.That(exists, Is.True);
        }

        [Test]
        public void Can_Update_ContentType_With_PropertyType_Removed()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);
            var contentType = repository.Get(1046);

            // Act
            var contentType2 = repository.Get(1046);
            contentType2.PropertyGroups["Meta"].PropertyTypes.Remove("metaDescription");
            repository.AddOrUpdate(contentType2);
            unitOfWork.Commit();

            var contentType3 = repository.Get(1046);

            // Assert
            Assert.That(contentType3.PropertyTypes.Any(x => x.Alias == "metaDescription"), Is.False);
            Assert.That(contentType.PropertyGroups.Count, Is.EqualTo(contentType3.PropertyGroups.Count));
            Assert.That(contentType.PropertyTypes.Count(), Is.EqualTo(contentType3.PropertyTypes.Count()));
        }

        [Test]
        public void Can_Verify_PropertyTypes_On_SimpleTextpage()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            // Act
            var contentType = repository.Get(1045);

            // Assert
            Assert.That(contentType.PropertyTypes.Count(), Is.EqualTo(3));
            Assert.That(contentType.PropertyGroups.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Can_Verify_PropertyTypes_On_Textpage()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            // Act
            var contentType = repository.Get(1046);

            // Assert
            Assert.That(contentType.PropertyTypes.Count(), Is.EqualTo(4));
            Assert.That(contentType.PropertyGroups.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Can_Verify_PropertyType_With_No_Group()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);
            var contentType = repository.Get(1046);

            // Act
            var urlAlias = new PropertyType(new Guid(), DataTypeDatabaseType.Nvarchar)
                               {
                                   Alias = "urlAlias",
                                   Name = "Url Alias",
                                   Description = "",
                                   HelpText = "",
                                   Mandatory = false,
                                   SortOrder = 1,
                                   DataTypeDefinitionId = -88
                               };
            
            var addedPropertyType = contentType.AddPropertyType(urlAlias);
            repository.AddOrUpdate(contentType);
            unitOfWork.Commit();

            // Assert
            var updated = repository.Get(1046);
            Assert.That(addedPropertyType, Is.True);
            Assert.That(updated.PropertyGroups.Count(), Is.EqualTo(2));
            Assert.That(updated.PropertyTypes.Count(), Is.EqualTo(5));
            Assert.That(updated.PropertyTypes.Any(x => x.Alias == "urlAlias"), Is.True);
            Assert.That(updated.PropertyTypes.First(x => x.Alias == "urlAlias").PropertyGroupId, Is.Null);
        }

        [Test]
        public void Can_Verify_AllowedChildContentTypes_On_ContentType()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);

            var subpageContentType = MockedContentTypes.CreateSimpleContentType("umbSubpage", "Subpage");
            var simpleSubpageContentType = MockedContentTypes.CreateSimpleContentType("umbSimpleSubpage", "Simple Subpage");
            repository.AddOrUpdate(subpageContentType);
            repository.AddOrUpdate(simpleSubpageContentType);
            unitOfWork.Commit();

            // Act
            var contentType = repository.Get(1045);
            contentType.AllowedContentTypes = new List<ContentTypeSort>
                                                  {
                                                      new ContentTypeSort
                                                          {
                                                              Alias = subpageContentType.Alias,
                                                              Id = new Lazy<int>(() => subpageContentType.Id),
                                                              SortOrder = 0
                                                          },
                                                      new ContentTypeSort
                                                          {
                                                              Alias = simpleSubpageContentType.Alias,
                                                              Id = new Lazy<int>(() => simpleSubpageContentType.Id),
                                                              SortOrder = 1
                                                          }
                                                  };
            repository.AddOrUpdate(contentType);
            unitOfWork.Commit();

            //Assert
            var updated = repository.Get(1045);

            Assert.That(updated.AllowedContentTypes.Any(), Is.True);
            Assert.That(updated.AllowedContentTypes.Any(x => x.Alias == subpageContentType.Alias), Is.True);
            Assert.That(updated.AllowedContentTypes.Any(x => x.Alias == simpleSubpageContentType.Alias), Is.True);
        }

        [Test]
        public void Can_Verify_Removal_Of_Used_PropertyType_From_ContentType()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);
            var contentRepository = RepositoryResolver.Current.ResolveByType<IContentRepository>(unitOfWork);
            var contentType = repository.Get(1046);

            var subpage = MockedContent.CreateTextpageContent(contentType, "Text Page 1", contentType.Id);
            contentRepository.AddOrUpdate(subpage);
            unitOfWork.Commit();

            // Act
            contentType.RemovePropertyType("keywords");
            repository.AddOrUpdate(contentType);
            unitOfWork.Commit();

            // Assert
            Assert.That(contentType.PropertyTypes.Count(), Is.EqualTo(3));
            Assert.That(contentType.PropertyTypes.Any(x => x.Alias == "keywords"), Is.False);
            Assert.That(subpage.Properties.First(x => x.Alias == "metaDescription").Value, Is.EqualTo("This is the meta description for a textpage"));
        }

        [Test]
        public void Can_Verify_Addition_Of_PropertyType_After_ContentType_Is_Used()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);
            var contentRepository = RepositoryResolver.Current.ResolveByType<IContentRepository>(unitOfWork);
            var contentType = repository.Get(1046);

            var subpage = MockedContent.CreateTextpageContent(contentType, "Text Page 1", contentType.Id);
            contentRepository.AddOrUpdate(subpage);
            unitOfWork.Commit();

            // Act
            var propertyGroup = contentType.PropertyGroups.First(x => x.Name == "Meta");
            propertyGroup.PropertyTypes.Add(new PropertyType(new Guid(), DataTypeDatabaseType.Ntext) { Alias = "metaAuthor", Name = "Meta Author", Description = "", HelpText = "", Mandatory = false, SortOrder = 1, DataTypeDefinitionId = -88 });
            repository.AddOrUpdate(contentType);
            unitOfWork.Commit();

            // Assert
            Assert.That(contentType.PropertyTypes.Count(), Is.EqualTo(5));
            Assert.That(contentType.PropertyTypes.Any(x => x.Alias == "metaAuthor"), Is.True);
        }

        [Test]
        public void Can_Verify_Usage_Of_New_PropertyType_On_Content()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);
            var contentRepository = RepositoryResolver.Current.ResolveByType<IContentRepository>(unitOfWork);
            var contentType = repository.Get(1046);

            var subpage = MockedContent.CreateTextpageContent(contentType, "Text Page 1", contentType.Id);
            contentRepository.AddOrUpdate(subpage);
            unitOfWork.Commit();

            var propertyGroup = contentType.PropertyGroups.First(x => x.Name == "Meta");
            propertyGroup.PropertyTypes.Add(new PropertyType(new Guid(), DataTypeDatabaseType.Ntext) { Alias = "metaAuthor", Name = "Meta Author", Description = "", HelpText = "", Mandatory = false, SortOrder = 1, DataTypeDefinitionId = -88 });
            repository.AddOrUpdate(contentType);
            unitOfWork.Commit();

            // Act
            var content = contentRepository.Get(subpage.Id);
            content.SetValue("metaAuthor", "John Doe");
            contentRepository.AddOrUpdate(content);
            unitOfWork.Commit();

            //Assert
            var updated = contentRepository.Get(subpage.Id);
            Assert.That(updated.GetValue("metaAuthor").ToString(), Is.EqualTo("John Doe"));
            Assert.That(contentType.PropertyTypes.Count(), Is.EqualTo(5));
            Assert.That(contentType.PropertyTypes.Any(x => x.Alias == "metaAuthor"), Is.True);
        }

        [Test]
        public void
            Can_Verify_That_A_Combination_Of_Adding_And_Deleting_PropertyTypes_Doesnt_Cause_Issues_For_Content_And_ContentType
            ()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = RepositoryResolver.Current.ResolveByType<IContentTypeRepository>(unitOfWork);
            var contentRepository = RepositoryResolver.Current.ResolveByType<IContentRepository>(unitOfWork);
            var contentType = repository.Get(1046);

            var subpage = MockedContent.CreateTextpageContent(contentType, "Text Page 1", contentType.Id);
            contentRepository.AddOrUpdate(subpage);
            unitOfWork.Commit();

            //Remove PropertyType
            contentType.RemovePropertyType("keywords");
            //Add PropertyType
            var propertyGroup = contentType.PropertyGroups.First(x => x.Name == "Meta");
            propertyGroup.PropertyTypes.Add(new PropertyType(new Guid(), DataTypeDatabaseType.Ntext) { Alias = "metaAuthor", Name = "Meta Author", Description = "", HelpText = "", Mandatory = false, SortOrder = 1, DataTypeDefinitionId = -88 });
            repository.AddOrUpdate(contentType);
            unitOfWork.Commit();

            // Act
            var content = contentRepository.Get(subpage.Id);
            content.SetValue("metaAuthor", "John Doe");
            contentRepository.AddOrUpdate(content);
            unitOfWork.Commit();

            //Assert
            var updated = contentRepository.Get(subpage.Id);
            Assert.That(updated.GetValue("metaAuthor").ToString(), Is.EqualTo("John Doe"));
            Assert.That(updated.Properties.First(x => x.Alias == "metaDescription").Value, Is.EqualTo("This is the meta description for a textpage"));

            Assert.That(contentType.PropertyTypes.Count(), Is.EqualTo(4));
            Assert.That(contentType.PropertyTypes.Any(x => x.Alias == "metaAuthor"), Is.True);
            Assert.That(contentType.PropertyTypes.Any(x => x.Alias == "keywords"), Is.False);
        }

        public void CreateTestData()
        {
            //Create and Save ContentType "umbTextpage" -> 1045
            ContentType simpleContentType = MockedContentTypes.CreateSimpleContentType("umbTextpage", "Textpage");
            ServiceContext.ContentTypeService.Save(simpleContentType);

            //Create and Save ContentType "textPage" -> 1046
            ContentType textpageContentType = MockedContentTypes.CreateTextpageContentType();
            ServiceContext.ContentTypeService.Save(textpageContentType);
        }
    }
}