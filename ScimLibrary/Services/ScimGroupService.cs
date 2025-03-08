using Newtonsoft.Json;
using ScimAPI.Repository;
using ScimAPI.Utilities;
using ScimLibrary.BusinessModels;
using ScimLibrary.Models;
using System.Reflection;

namespace ScimLibrary.Services
{
    public class ScimGroupService
    {
        IScimRepository<ScimGroup> repository;
        public ScimGroupService(IScimRepository<ScimGroup> repository)
        {
            this.repository = repository;
        }
        public async Task<ScimGroup> AddGroup(ScimGroup group)
        {
            await repository.AddAsync(group);
            group.Id = group.ExternalId;
            return group;
        }

        public void DeleteGroup(ScimGroup group)
        {
            repository.DeleteAsync(group.ExternalId);
        }

        public async Task<ScimGroup> GetGroupById(string externalId)
        {
            return await repository.GetByIdAsync(externalId);
        }

        public async Task<ScimListResponse<ScimGroup>> GetGroupByFilter(string filter)
        {
            var allGroups = await repository.GetAllAsync();
            var parsedFilter = ScimFilter.Parse(filter);
            var predicate = parsedFilter.ToLambda<ScimGroup>();
            var matchedGroups = allGroups.Where(predicate).ToList();

            return new ScimListResponse<ScimGroup>() { Resources = matchedGroups };
        }

        public async Task<bool> PatchGroupAsync(string groupId, ScimPatchOperation patchOperations)
        {
            var group = await repository.GetByIdAsync(groupId);
            if (group == null) return false;

            foreach (var operation in patchOperations.Operations)
            {
                if (string.Equals(operation.Op, "replace", StringComparison.OrdinalIgnoreCase) && operation.Path != null)
                {
                    var property = typeof(ScimGroup).GetProperty(operation.Path, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(group, Convert.ChangeType(operation.Value.ToString(), property.PropertyType));
                    }
                }
                else if (string.Equals(operation.Op, "add", StringComparison.OrdinalIgnoreCase) && operation.Path != null)
                {
                    var property = typeof(ScimGroup).GetProperty(operation.Path, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null && property.CanWrite)
                    {
                        var propertyType = property.PropertyType;

                        if (propertyType == typeof(IEnumerable<ScimMember>))
                        {
                            var members = JsonConvert.DeserializeObject<List<ScimMember>>(operation.Value.ToString());
                            property.SetValue(group, members);
                        }
                        else
                        {
                            var convertedValue = JsonConvert.DeserializeObject(operation.Value.ToString(), propertyType);
                            property.SetValue(group, convertedValue);
                        }
                    }
                }
                else if (string.Equals(operation.Op, "remove", StringComparison.OrdinalIgnoreCase) && operation.Path != null)
                {
                    var property = typeof(ScimGroup).GetProperty(operation.Path, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(group, null);
                    }
                }
            }

            await repository.UpdateAsync(group);
            return true;
        }
    }
}
