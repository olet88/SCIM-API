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
        IScimGroupRepository repository;
        public ScimGroupService(IScimGroupRepository repository)
        {
            this.repository = repository;
        }
        public async Task<ScimGroup> AddGroup(ScimGroup group)
        {
            await repository.AddGroupAsync(group);
            group.Id = group.ExternalId;
            return group;
        }

        public void DeleteGroup(ScimGroup group)
        {
            repository.DeleteGroupAsync(group.ExternalId);
        }

        public async Task<ScimGroup> GetGroupById(string externalId)
        {
            return await repository.GetGroupByIdAsync(externalId);
        }

        public async Task<ScimListResponse<ScimGroup>> GetGroupByFilter(string filter)
        {
            var allGroups = await repository.GetAllGroupsAsync();
            var parsedFilter = ScimFilter.Parse(filter);
            var predicate = parsedFilter.ToLambda<ScimGroup>();
            var matchedGroups = allGroups.Where(predicate).ToList();

            return new ScimListResponse<ScimGroup>() { Resources = matchedGroups };
        }

        public async Task<bool> PatchGroupAsync(string groupId, ScimPatchOperation patchOperations)
        {
            var group = await repository.GetGroupByIdAsync(groupId);
            if (group == null) return false;

            foreach (var operation in patchOperations.Operations)
            {
                if (string.IsNullOrEmpty(operation.Path)) continue;

                var property = typeof(ScimGroup).GetProperty(
                    operation.Path,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                );

                if (property == null || !property.CanWrite) continue;

                switch (operation.Op.ToLowerInvariant())
                {
                    case "replace":
                    case "add":
                        if (property.PropertyType == typeof(IEnumerable<ScimMember>))
                        {
                            var members = JsonConvert.DeserializeObject<List<ScimMember>>(operation.Value.ToString());
                            property.SetValue(group, members);
                        }
                        else
                        {
                            var convertedValue = operation.Value.ToString();
                            var toString2 = convertedValue.ToString();
                            property.SetValue(group, convertedValue);
                        }
                        break;

                    case "remove":
                        property.SetValue(group, null);
                        break;
                }
            }

            await repository.UpdateGroupAsync(group);
            return true;
        }
    }
}
