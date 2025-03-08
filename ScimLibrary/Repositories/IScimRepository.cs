using ScimLibrary.BusinessModels;
using ScimLibrary.Models;
using System.Collections.Concurrent;

namespace ScimAPI.Repository
{
    public interface IScimRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task ReplaceAsync(T entity);
        Task DeleteAsync(string id);
    }

}
