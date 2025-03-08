using ScimAPI.Repository;
using ScimLibrary.Models;
using System.Collections.Concurrent;

namespace ScimAPI.Demo
{
    internal class InMemoryDatabaseRepository : IScimUserRepository, IScimGroupRepository
    {
        // Quick and dirty in-memory database and demo repository. DO NOT use in production -- or for any other purpose than learning how SCIM works.

        public static readonly ConcurrentDictionary<string, ScimUser> InMemoryUserDatabase = new ConcurrentDictionary<string, ScimUser>();
        public static readonly ConcurrentDictionary<string, ScimGroup> InMemoryGroupDatabase = new ConcurrentDictionary<string, ScimGroup>();


        public Task AddUserAsync(ScimUser entity)
        {
            InMemoryUserDatabase.TryAdd(entity.ExternalId, entity);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ScimUser>> GetAllUsersAsync()
        {
            return Task.FromResult<IEnumerable<ScimUser>>(InMemoryUserDatabase.Values.ToList());
        }

        public Task<ScimUser> GetUserByIdAsync(string id)
        {
            if (!InMemoryUserDatabase.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }

            return Task.FromResult(InMemoryUserDatabase[id]);
        }

        public Task DeleteUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceUserAsync(ScimUser entity)
        {
            if (InMemoryUserDatabase.ContainsKey(entity.ExternalId))
            {
                InMemoryUserDatabase[entity.ExternalId] = entity;
                return Task.CompletedTask;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }


        public Task UpdateUserAsync(ScimUser entity)
        {
            if (InMemoryUserDatabase.ContainsKey(entity.ExternalId))
            {
                InMemoryUserDatabase[entity.ExternalId] = entity;
                return Task.CompletedTask;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public Task AddGroupAsync(ScimGroup entity)
        {
            InMemoryGroupDatabase.TryAdd(entity.ExternalId, entity);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ScimGroup>> GetAllGroupsAsync()
        {
            return Task.FromResult<IEnumerable<ScimGroup>>(InMemoryGroupDatabase.Values.ToList());
        }

        public Task<ScimGroup> GetGroupByIdAsync(string id)
        {
            if (!InMemoryGroupDatabase.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }

            return Task.FromResult(InMemoryGroupDatabase[id]);
        }

        public Task DeleteGroupAsync(string id)
        {
            InMemoryGroupDatabase.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task ReplaceGroupAsync(ScimGroup entity)
        {
            if (InMemoryUserDatabase.ContainsKey(entity.ExternalId))
            {
                InMemoryGroupDatabase[entity.ExternalId] = entity;
                return Task.CompletedTask;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public Task UpdateGroupAsync(ScimGroup entity)
        {
            if (InMemoryGroupDatabase.ContainsKey(entity.ExternalId))
            {
                InMemoryGroupDatabase[entity.ExternalId] = entity;
                return Task.CompletedTask;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        } 
    }
}
