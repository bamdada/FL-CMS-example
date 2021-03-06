using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Umbraco.Core.Services
{
    public class RelationService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly EntityService _entityService;

        public RelationService(IDatabaseUnitOfWorkProvider uowProvider, RepositoryFactory repositoryFactory,
                               EntityService entityService)
        {
            _uowProvider = uowProvider;
            _repositoryFactory = repositoryFactory;
            _entityService = entityService;
        }

        /// <summary>
        /// Gets a <see cref="Relation"/> by its Id
        /// </summary>
        /// <param name="id">Id of the <see cref="Relation"/></param>
        /// <returns>A <see cref="Relation"/> object</returns>
        public Relation GetById(int id)
        {
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a <see cref="RelationType"/> by its Id
        /// </summary>
        /// <param name="id">Id of the <see cref="RelationType"/></param>
        /// <returns>A <see cref="RelationType"/> object</returns>
        public RelationType GetRelationTypeById(int id)
        {
            using (var repository = _repositoryFactory.CreateRelationTypeRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(id);
            }
        }

        /// <summary>
        /// Gets a <see cref="RelationType"/> by its Alias
        /// </summary>
        /// <param name="alias">Alias of the <see cref="RelationType"/></param>
        /// <returns>A <see cref="RelationType"/> object</returns>
        public RelationType GetRelationTypeByAlias(string alias)
        {
            using (var repository = _repositoryFactory.CreateRelationTypeRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<RelationType>().Where(x => x.Alias == alias);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets all <see cref="Relation"/> objects
        /// </summary>
        /// <param name="ids">Optional array of integer ids to return relations for</param>
        /// <returns>An enumerable list of <see cref="Relation"/> objects</returns>
        public IEnumerable<Relation> GetAllRelations(params int[] ids)
        {
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids);
            }
        }

        /// <summary>
        /// Gets all <see cref="Relation"/> objects by their <see cref="RelationType"/>
        /// </summary>
        /// <param name="relationType"><see cref="RelationType"/> to retrieve Relations for</param>
        /// <returns>An enumerable list of <see cref="Relation"/> objects</returns>
        public IEnumerable<Relation> GetAllRelationsByRelationType(RelationType relationType)
        {
            return GetAllRelationsByRelationType(relationType.Id);
        }

        /// <summary>
        /// Gets all <see cref="Relation"/> objects by their <see cref="RelationType"/>'s Id
        /// </summary>
        /// <param name="relationTypeId">Id of the <see cref="RelationType"/> to retrieve Relations for</param>
        /// <returns>An enumerable list of <see cref="Relation"/> objects</returns>
        public IEnumerable<Relation> GetAllRelationsByRelationType(int relationTypeId)
        {
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<Relation>().Where(x => x.RelationTypeId == relationTypeId);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets all <see cref="Relation"/> objects
        /// </summary>
        /// <param name="ids">Optional array of integer ids to return relationtypes for</param>
        /// <returns>An enumerable list of <see cref="RelationType"/> objects</returns>
        public IEnumerable<RelationType> GetAllRelationTypes(params int[] ids)
        {
            using (var repository = _repositoryFactory.CreateRelationTypeRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(ids);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="Relation"/> objects by their parent Id
        /// </summary>
        /// <param name="id">Id of the parent to retrieve relations for</param>
        /// <returns>An enumerable list of <see cref="Relation"/> objects</returns>
        public IEnumerable<Relation> GetByParentId(int id)
        {
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<Relation>().Where(x => x.ParentId == id);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="Relation"/> objects by their child Id
        /// </summary>
        /// <param name="id">Id of the child to retrieve relations for</param>
        /// <returns>An enumerable list of <see cref="Relation"/> objects</returns>
        public IEnumerable<Relation> GetByChildId(int id)
        {
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<Relation>().Where(x => x.ChildId == id);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="Relation"/> objects by their child or parent Id.
        /// Using this method will get you all relations regards of it being a child or parent relation.
        /// </summary>
        /// <param name="id">Id of the child or parent to retrieve relations for</param>
        /// <returns>An enumerable list of <see cref="Relation"/> objects</returns>
        public IEnumerable<Relation> GetByParentOrChildId(int id)
        {
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<Relation>().Where(x => x.ChildId == id || x.ParentId == id);
                return repository.GetByQuery(query);
            }
        }
        
        /// <summary>
        /// Gets a list of <see cref="Relation"/> objects by the Name of the <see cref="RelationType"/>
        /// </summary>
        /// <param name="relationTypeName">Name of the <see cref="RelationType"/> to retrieve Relations for</param>
        /// <returns>An enumerable list of <see cref="Relation"/> objects</returns>
        public IEnumerable<Relation> GetByRelationTypeName(string relationTypeName)
        {
            List<int> relationTypeIds = null;
            using (var repository = _repositoryFactory.CreateRelationTypeRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<RelationType>().Where(x => x.Name == relationTypeName);
                var relationTypes = repository.GetByQuery(query);
                if (relationTypes.Any())
                {
                    relationTypeIds = relationTypes.Select(x => x.Id).ToList();
                }
            }

            if (relationTypeIds == null)
                return Enumerable.Empty<Relation>();

            return GetRelationsByListOfTypeIds(relationTypeIds);
        }

        /// <summary>
        /// Gets a list of <see cref="Relation"/> objects by the Alias of the <see cref="RelationType"/>
        /// </summary>
        /// <param name="relationTypeAlias">Alias of the <see cref="RelationType"/> to retrieve Relations for</param>
        /// <returns>An enumerable list of <see cref="Relation"/> objects</returns>
        public IEnumerable<Relation> GetByRelationTypeAlias(string relationTypeAlias)
        {
            List<int> relationTypeIds = null;
            using (var repository = _repositoryFactory.CreateRelationTypeRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<RelationType>().Where(x => x.Alias == relationTypeAlias);
                var relationTypes = repository.GetByQuery(query);
                if (relationTypes.Any())
                {
                    relationTypeIds = relationTypes.Select(x => x.Id).ToList();
                }
            }

            if (relationTypeIds == null)
                return Enumerable.Empty<Relation>();

            return GetRelationsByListOfTypeIds(relationTypeIds);
        }

        /// <summary>
        /// Gets a list of <see cref="Relation"/> objects by the Id of the <see cref="RelationType"/>
        /// </summary>
        /// <param name="relationTypeId">Id of the <see cref="RelationType"/> to retrieve Relations for</param>
        /// <returns>An enumerable list of <see cref="Relation"/> objects</returns>
        public IEnumerable<Relation> GetByRelationTypeId(int relationTypeId)
        {
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<Relation>().Where(x => x.RelationTypeId == relationTypeId);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets the Child object from a Relation as an <see cref="IUmbracoEntity"/>
        /// </summary>
        /// <param name="relation">Relation to retrieve child object from</param>
        /// <param name="loadBaseType">Optional bool to load the complete object graph when set to <c>False</c></param>
        /// <returns>An <see cref="IUmbracoEntity"/></returns>
        public IUmbracoEntity GetChildEntityFromRelation(Relation relation, bool loadBaseType = false)
        {
            var objectType = UmbracoObjectTypesExtensions.GetUmbracoObjectType(relation.RelationType.ChildObjectType);
            return _entityService.Get(relation.ChildId, objectType, loadBaseType);
        }

        /// <summary>
        /// Gets the Parent object from a Relation as an <see cref="IUmbracoEntity"/>
        /// </summary>
        /// <param name="relation">Relation to retrieve parent object from</param>
        /// <param name="loadBaseType">Optional bool to load the complete object graph when set to <c>False</c></param>
        /// <returns>An <see cref="IUmbracoEntity"/></returns>
        public IUmbracoEntity GetParentEntityFromRelation(Relation relation, bool loadBaseType = false)
        {
            var objectType = UmbracoObjectTypesExtensions.GetUmbracoObjectType(relation.RelationType.ParentObjectType);
            return _entityService.Get(relation.ParentId, objectType, loadBaseType);
        }

        /// <summary>
        /// Gets the Parent and Child objects from a Relation as a <see cref="Tuple"/>"/> with <see cref="IUmbracoEntity"/>.
        /// </summary>
        /// <param name="relation">Relation to retrieve parent and child object from</param>
        /// <param name="loadBaseType">Optional bool to load the complete object graph when set to <c>False</c></param>
        /// <returns>Returns a Tuple with Parent (item1) and Child (item2)</returns>
        public Tuple<IUmbracoEntity, IUmbracoEntity> GetEntitiesFromRelation(Relation relation, bool loadBaseType = false)
        {
            var childObjectType = UmbracoObjectTypesExtensions.GetUmbracoObjectType(relation.RelationType.ChildObjectType);
            var parentObjectType = UmbracoObjectTypesExtensions.GetUmbracoObjectType(relation.RelationType.ParentObjectType);

            var child = _entityService.Get(relation.ChildId, childObjectType, loadBaseType);
            var parent = _entityService.Get(relation.ParentId, parentObjectType, loadBaseType);

            return new Tuple<IUmbracoEntity, IUmbracoEntity>(parent, child);
        }

        /// <summary>
        /// Gets the Child objects from a list of Relations as a list of <see cref="IUmbracoEntity"/> objects.
        /// </summary>
        /// <param name="relations">List of relations to retrieve child objects from</param>
        /// <param name="loadBaseType">Optional bool to load the complete object graph when set to <c>False</c></param>
        /// <returns>An enumerable list of <see cref="IUmbracoEntity"/></returns>
        public IEnumerable<IUmbracoEntity> GetChildEntitiesFromRelations(IEnumerable<Relation> relations, bool loadBaseType = false)
        {
            foreach (var relation in relations)
            {
                var objectType = UmbracoObjectTypesExtensions.GetUmbracoObjectType(relation.RelationType.ChildObjectType);
                yield return _entityService.Get(relation.ChildId, objectType, loadBaseType);
            }
        }

        /// <summary>
        /// Gets the Parent objects from a list of Relations as a list of <see cref="IUmbracoEntity"/> objects.
        /// </summary>
        /// <param name="relations">List of relations to retrieve parent objects from</param>
        /// <param name="loadBaseType">Optional bool to load the complete object graph when set to <c>False</c></param>
        /// <returns>An enumerable list of <see cref="IUmbracoEntity"/></returns>
        public IEnumerable<IUmbracoEntity> GetParentEntitiesFromRelations(IEnumerable<Relation> relations,
                                                                          bool loadBaseType = false)
        {
            foreach (var relation in relations)
            {
                var objectType = UmbracoObjectTypesExtensions.GetUmbracoObjectType(relation.RelationType.ParentObjectType);
                yield return _entityService.Get(relation.ParentId, objectType, loadBaseType);
            }
        }

        /// <summary>
        /// Gets the Parent and Child objects from a list of Relations as a list of <see cref="IUmbracoEntity"/> objects.
        /// </summary>
        /// <param name="relations">List of relations to retrieve parent and child objects from</param>
        /// <param name="loadBaseType">Optional bool to load the complete object graph when set to <c>False</c></param>
        /// <returns>An enumerable list of <see cref="Tuple"/> with <see cref="IUmbracoEntity"/></returns>
        public IEnumerable<Tuple<IUmbracoEntity, IUmbracoEntity>> GetEntitiesFromRelations(
            IEnumerable<Relation> relations,
            bool loadBaseType = false)
        {
            foreach (var relation in relations)
            {
                var childObjectType = UmbracoObjectTypesExtensions.GetUmbracoObjectType(relation.RelationType.ChildObjectType);
                var parentObjectType = UmbracoObjectTypesExtensions.GetUmbracoObjectType(relation.RelationType.ParentObjectType);

                var child = _entityService.Get(relation.ChildId, childObjectType, loadBaseType);
                var parent = _entityService.Get(relation.ParentId, parentObjectType, loadBaseType);

                yield return new Tuple<IUmbracoEntity, IUmbracoEntity>(parent, child);
            }
        }
        
        /// <summary>
        /// Relates two objects that are based on the <see cref="IUmbracoEntity"/> interface.
        /// </summary>
        /// <param name="parent">Parent entity</param>
        /// <param name="child">Child entity</param>
        /// <param name="relationType">The type of relation to create</param>
        /// <returns>The created <see cref="Relation"/></returns>
        public Relation Relate(IUmbracoEntity parent, IUmbracoEntity child, RelationType relationType)
        {
            //Ensure that the RelationType has an indentity before using it to relate two entities
            if(relationType.HasIdentity == false)
                Save(relationType);

            var relation = new Relation(parent.Id, child.Id, relationType);
            var uow = _uowProvider.GetUnitOfWork();
            using (var repository = _repositoryFactory.CreateRelationRepository(uow))
            {
                repository.AddOrUpdate(relation);
                uow.Commit();

                return relation;
            }
        }

        /// <summary>
        /// Relates two objects that are based on the <see cref="IUmbracoEntity"/> interface.
        /// </summary>
        /// <param name="parent">Parent entity</param>
        /// <param name="child">Child entity</param>
        /// <param name="relationTypeAlias">Alias of the type of relation to create</param>
        /// <returns>The created <see cref="Relation"/></returns>
        public Relation Relate(IUmbracoEntity parent, IUmbracoEntity child, string relationTypeAlias)
        {
            var relationType = GetRelationTypeByAlias(relationTypeAlias);
            if(relationType == null || string.IsNullOrEmpty(relationType.Alias))
                throw new ArgumentNullException(string.Format("No RelationType with Alias '{0}' exists.", relationTypeAlias));

            var relation = new Relation(parent.Id, child.Id, relationType);
            var uow = _uowProvider.GetUnitOfWork();
            using (var repository = _repositoryFactory.CreateRelationRepository(uow))
            {
                repository.AddOrUpdate(relation);
                uow.Commit();

                return relation;
            }
        }

        /// <summary>
        /// Checks whether any relations exists for the passed in <see cref="RelationType"/>.
        /// </summary>
        /// <param name="relationType"><see cref="RelationType"/> to check for relations</param>
        /// <returns>Returns <c>True</c> if any relations exists for the given <see cref="RelationType"/>, otherwise <c>False</c></returns>
        public bool HasRelations(RelationType relationType)
        {
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<Relation>().Where(x => x.RelationTypeId == relationType.Id);
                return repository.GetByQuery(query).Any();
            }
        }

        /// <summary>
        /// Checks whether any relations exists for the passed in Id.
        /// </summary>
        /// <param name="id">Id of an object to check relations for</param>
        /// <returns>Returns <c>True</c> if any relations exists with the given Id, otherwise <c>False</c></returns>
        public bool IsRelated(int id)
        {
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = new Query<Relation>().Where(x => x.ParentId == id || x.ChildId == id);
                return repository.GetByQuery(query).Any();
            }
        }

        /// <summary>
        /// Saves a <see cref="Relation"/>
        /// </summary>
        /// <param name="relation">Relation to save</param>
        public void Save(Relation relation)
        {
            var uow = _uowProvider.GetUnitOfWork();
            using (var repository = _repositoryFactory.CreateRelationRepository(uow))
            {
                repository.AddOrUpdate(relation);
                uow.Commit();
            }
        }

        /// <summary>
        /// Saves a <see cref="RelationType"/>
        /// </summary>
        /// <param name="relationType">RelationType to Save</param>
        public void Save(RelationType relationType)
        {
            var uow = _uowProvider.GetUnitOfWork();
            using (var repository = _repositoryFactory.CreateRelationTypeRepository(uow))
            {
                repository.AddOrUpdate(relationType);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes a <see cref="Relation"/>
        /// </summary>
        /// <param name="relation">Relation to Delete</param>
        public void Delete(Relation relation)
        {
            var uow = _uowProvider.GetUnitOfWork();
            using (var repository = _repositoryFactory.CreateRelationRepository(uow))
            {
                repository.Delete(relation);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes a <see cref="RelationType"/>
        /// </summary>
        /// <param name="relationType">RelationType to Delete</param>
        public void Delete(RelationType relationType)
        {
            var uow = _uowProvider.GetUnitOfWork();
            using (var repository = _repositoryFactory.CreateRelationTypeRepository(uow))
            {
                repository.Delete(relationType);
                uow.Commit();
            }
        }

        /// <summary>
        /// Deletes all <see cref="Relation"/> objects based on the passed in <see cref="RelationType"/>
        /// </summary>
        /// <param name="relationType"><see cref="RelationType"/> to Delete Relations for</param>
        public void DeleteRelationsOfType(RelationType relationType)
        {
            var uow = _uowProvider.GetUnitOfWork();
            using (var repository = _repositoryFactory.CreateRelationRepository(uow))
            {
                var query = new Query<Relation>().Where(x => x.RelationTypeId == relationType.Id);
                var list = repository.GetByQuery(query).ToList();

                foreach (var relation in list)
                {
                    repository.Delete(relation);
                }
                uow.Commit();
            }
        }

        #region Private Methods
        private IEnumerable<Relation> GetRelationsByListOfTypeIds(IEnumerable<int> relationTypeIds)
        {
            var relations = new List<Relation>();
            using (var repository = _repositoryFactory.CreateRelationRepository(_uowProvider.GetUnitOfWork()))
            {
                foreach (var relationTypeId in relationTypeIds)
                {
                    int id = relationTypeId;
                    var query = new Query<Relation>().Where(x => x.RelationTypeId == id);
                    relations.AddRange(repository.GetByQuery(query).ToList());
                }
            }
            return relations;
        }
        #endregion
    }
}