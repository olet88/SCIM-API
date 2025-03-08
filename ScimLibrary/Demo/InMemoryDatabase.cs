using ScimAPI.Repository;
using ScimLibrary.Models;
using System.Collections.Concurrent;

namespace ScimAPI.Demo
{
    public class InMemoryRepository : IScimRepository<ScimUser>, IScimRepository<ScimGroup>
    {
        // Quick and dirty in-memory database and demo repository. DO NOT use in production -- or for any other purpose than learning how SCIM works.

        public static readonly ConcurrentDictionary<string, ScimUser> InMemoryUserDatabase = new ConcurrentDictionary<string, ScimUser>();
        public static readonly ConcurrentDictionary<string, ScimGroup> InMemoryGroupDatabase = new ConcurrentDictionary<string, ScimGroup>();


        public Task AddAsync(ScimUser entity)
        {
            InMemoryUserDatabase.TryAdd(entity.ExternalId, entity);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ScimUser>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<ScimUser>>(InMemoryUserDatabase.Values.ToList());
        }

        public Task<ScimUser> GetByIdAsync(string id)
        {
            if (!InMemoryUserDatabase.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }

            return Task.FromResult(InMemoryUserDatabase[id]);
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceAsync(ScimUser entity)
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


        public Task UpdateAsync(ScimUser entity)
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

        Task IScimRepository<ScimGroup>.AddAsync(ScimGroup entity)
        {
            InMemoryGroupDatabase.TryAdd(entity.ExternalId, entity);
            return Task.CompletedTask;
        }

        Task<IEnumerable<ScimGroup>> IScimRepository<ScimGroup>.GetAllAsync()
        {
            return Task.FromResult<IEnumerable<ScimGroup>>(InMemoryGroupDatabase.Values.ToList());
        }

        Task<ScimGroup> IScimRepository<ScimGroup>.GetByIdAsync(string id)
        {
            if (!InMemoryGroupDatabase.ContainsKey(id))
            {
                throw new KeyNotFoundException();
            }

            return Task.FromResult(InMemoryGroupDatabase[id]);
        }

        Task IScimRepository<ScimGroup>.DeleteAsync(string id)
        {
            InMemoryGroupDatabase.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        Task IScimRepository<ScimGroup>.ReplaceAsync(ScimGroup entity)
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


        Task IScimRepository<ScimGroup>.UpdateAsync(ScimGroup entity)
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
