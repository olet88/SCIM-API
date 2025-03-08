using ScimLibrary.BusinessModels;
using ScimLibrary.Models;
using System.Collections.Concurrent;

namespace ScimAPI.Repository
{
    public interface IScimUserRepository
    {
        Task<ScimUser> GetUserByIdAsync(string userId);
        Task<IEnumerable<ScimUser>> GetAllUsersAsync();
        Task AddUserAsync(ScimUser scimUser);
        Task UpdateUserAsync(ScimUser scimUser);
        Task ReplaceUserAsync(ScimUser scimUser);
        Task DeleteUserAsync(string userId);
    }

    public interface IScimGroupRepository
    {
        Task<ScimGroup> GetGroupByIdAsync(string id);
        Task<IEnumerable<ScimGroup>> GetAllGroupsAsync();
        Task AddGroupAsync(ScimGroup scimGroup);
        Task UpdateGroupAsync(ScimGroup scimGroup);
        Task ReplaceGroupAsync(ScimGroup scimGroup);
        Task DeleteGroupAsync(string id);
    }

}
