using System.Linq;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.Repositories;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Tests.TestHelpers;

namespace Umbraco.Tests.Persistence.Repositories
{
    [TestFixture]
    public class LanguageRepositoryTest : BaseDatabaseFactoryTest
    {
        [SetUp]
        public override void Initialize()
        {
            base.Initialize();

            CreateTestData();
        }

        [Test]
        public void Can_Instantiate_Repository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();

            // Act
            var repository = new LanguageRepository(unitOfWork);

            // Assert
            Assert.That(repository, Is.Not.Null);
        }

        [Test]
        public void Can_Perform_Get_On_LanguageRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = new LanguageRepository(unitOfWork);

            // Act
            var language = repository.Get(1);

            // Assert
            Assert.That(language, Is.Not.Null);
            Assert.That(language.HasIdentity, Is.True);
            Assert.That(language.CultureName, Is.EqualTo("en-US"));
            Assert.That(language.IsoCode, Is.EqualTo("en-US"));
        }

        [Test]
        public void Can_Perform_GetAll_On_LanguageRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = new LanguageRepository(unitOfWork);

            // Act
            var languages = repository.GetAll();

            // Assert
            Assert.That(languages, Is.Not.Null);
            Assert.That(languages.Any(), Is.True);
            Assert.That(languages.Any(x => x == null), Is.False);
            Assert.That(languages.Count(), Is.EqualTo(5));
        }

        [Test]
        public void Can_Perform_GetAll_With_Params_On_LanguageRepository()
        { 
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = new LanguageRepository(unitOfWork);

            // Act
            var languages = repository.GetAll(1, 2);

            // Assert
            Assert.That(languages, Is.Not.Null);
            Assert.That(languages.Any(), Is.True);
            Assert.That(languages.Any(x => x == null), Is.False);
            Assert.That(languages.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Can_Perform_GetByQuery_On_LanguageRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = new LanguageRepository(unitOfWork);

            // Act
            var query = Query<ILanguage>.Builder.Where(x => x.IsoCode == "da-DK");
            var result = repository.GetByQuery(query);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.True);
            Assert.That(result.FirstOrDefault().CultureName, Is.EqualTo("da-DK"));
        }

        [Test]
        public void Can_Perform_Count_On_LanguageRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = new LanguageRepository(unitOfWork);

            // Act
            var query = Query<ILanguage>.Builder.Where(x => x.IsoCode.StartsWith("D"));
            int count = repository.Count(query);

            // Assert
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Can_Perform_Add_On_LanguageRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = new LanguageRepository(unitOfWork);

            // Act
            var languageBR = new Language("pt-BR") { CultureName = "pt-BR" };
            repository.AddOrUpdate(languageBR);
            unitOfWork.Commit();

            // Assert
            Assert.That(languageBR.HasIdentity, Is.True);
            Assert.That(languageBR.Id, Is.EqualTo(6));//With 5 existing entries the Id should be 6
        }

        [Test]
        public void Can_Perform_Update_On_LanguageRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = new LanguageRepository(unitOfWork);

            // Act
            var language = repository.Get(5);
            language.IsoCode = "pt-BR";
            language.CultureName = "pt-BR";

            repository.AddOrUpdate(language);
            unitOfWork.Commit();

            var languageUpdated = repository.Get(5);

            // Assert
            Assert.That(languageUpdated, Is.Not.Null);
            Assert.That(languageUpdated.IsoCode, Is.EqualTo("pt-BR"));
            Assert.That(languageUpdated.CultureName, Is.EqualTo("pt-BR"));
        }

        [Test]
        public void Can_Perform_Delete_On_LanguageRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = new LanguageRepository(unitOfWork);

            // Act
            var language = repository.Get(3);
            repository.Delete(language);
            unitOfWork.Commit();

            var exists = repository.Exists(3);

            // Assert
            Assert.That(exists, Is.False);
        }

        [Test]
        public void Can_Perform_Exists_On_LanguageRepository()
        {
            // Arrange
            var provider = new PetaPocoUnitOfWorkProvider();
            var unitOfWork = provider.GetUnitOfWork();
            var repository = new LanguageRepository(unitOfWork);

            // Act
            var exists = repository.Exists(3);
            var doesntExist = repository.Exists(10);

            // Assert
            Assert.That(exists, Is.True);
            Assert.That(doesntExist, Is.False);
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        public void CreateTestData()
        {
            var languageDK = new Language("da-DK") { CultureName = "da-DK" };
            ServiceContext.LocalizationService.Save(languageDK);//Id 2

            var languageSE = new Language("sv-SE") { CultureName = "sv-SE" };
            ServiceContext.LocalizationService.Save(languageSE);//Id 3

            var languageDE = new Language("de-DE") { CultureName = "de-DE" };
            ServiceContext.LocalizationService.Save(languageDE);//Id 4

            var languagePT = new Language("pt-PT") { CultureName = "pt-PT" };
            ServiceContext.LocalizationService.Save(languagePT);//Id 5
        }
    }
}