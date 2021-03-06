using Umbraco.Core.Models;
using Umbraco.Core.Models.Rdbms;

namespace Umbraco.Core.Persistence.Factories
{
    internal class RelationTypeFactory : IEntityFactory<RelationType, RelationTypeDto>
    {
        #region Implementation of IEntityFactory<RelationType,RelationTypeDto>

        public RelationType BuildEntity(RelationTypeDto dto)
        {
            var entity = new RelationType(dto.ChildObjectType, dto.ParentObjectType, dto.Alias)
                             {
                                 Id = dto.Id,
                                 IsBidirectional = dto.Dual,
                                 Name = dto.Name
                             };
            //on initial construction we don't want to have dirty properties tracked
            // http://issues.umbraco.org/issue/U4-1946
            entity.ResetDirtyProperties(false);
            return entity;
        }

        public RelationTypeDto BuildDto(RelationType entity)
        {
            var dto = new RelationTypeDto
                          {
                              Alias = entity.Alias,
                              ChildObjectType = entity.ChildObjectType,
                              Dual = entity.IsBidirectional,
                              Name = entity.Name,
                              ParentObjectType = entity.ParentObjectType
                          };
            if(entity.HasIdentity)
                dto.Id = entity.Id;

            return dto;
        }

        #endregion
    }
}