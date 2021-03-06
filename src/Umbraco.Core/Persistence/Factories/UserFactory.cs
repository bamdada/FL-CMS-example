using System.Collections.Generic;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Models.Rdbms;

namespace Umbraco.Core.Persistence.Factories
{
    internal class UserFactory : IEntityFactory<IUser, UserDto>
    {
        private readonly IUserType _userType;

        public UserFactory(IUserType userType)
        {
            _userType = userType;
        }

        #region Implementation of IEntityFactory<IUser,UserDto>

        public IUser BuildEntity(UserDto dto)
        {
            var user = new User(_userType)
                       {
                           Id = dto.Id,
                           ProfileId = dto.Id,
                           StartContentId = dto.ContentStartId,
                           StartMediaId = dto.MediaStartId.HasValue ? dto.MediaStartId.Value : -1,
                           Password = dto.Password,
                           Username = dto.Login,
                           Name = dto.UserName,
                           IsLockedOut = dto.Disabled,
                           IsApproved = dto.Disabled == false,
                           Email = dto.Email,
                           Language = dto.UserLanguage,
                           DefaultToLiveEditing = dto.DefaultToLiveEditing,
                           NoConsole = dto.NoConsole,
                           DefaultPermissions = dto.DefaultPermissions
                       };

            foreach (var app in dto.User2AppDtos)
            {
                user.AddAllowedSection(app.AppAlias);
            }

            //on initial construction we don't want to have dirty properties tracked
            // http://issues.umbraco.org/issue/U4-1946
            user.ResetDirtyProperties(false);

            return user;
        }

        public UserDto BuildDto(IUser entity)
        {
            var dto = new UserDto
                          {
                              ContentStartId = entity.StartContentId,
                              MediaStartId = entity.StartMediaId,
                              DefaultToLiveEditing = entity.DefaultToLiveEditing,
                              Disabled = entity.IsApproved == false,
                              Email = entity.Email,
                              Login = entity.Username,
                              NoConsole = entity.NoConsole,
                              Password = entity.Password,
                              UserLanguage = entity.Language,
                              UserName = entity.Name,
                              Type = short.Parse(entity.UserType.Id.ToString()),
                              DefaultPermissions = entity.DefaultPermissions,
                              User2AppDtos = new List<User2AppDto>()
                          };

            foreach (var app in entity.AllowedSections)
            {
                var appDto = new User2AppDto
                    {
                        AppAlias = app
                    };
                if (entity.Id != null)
                {
                    appDto.UserId = (int) entity.Id;
                }

                dto.User2AppDtos.Add(appDto);
            }

            if (entity.HasIdentity)
                dto.Id = entity.Id.SafeCast<int>();

            return dto;
        }

        #endregion
    }
}