using ScimLibrary.Models;
using System.Reflection;
using ScimLibrary.BusinessModels;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using ScimAPI.Repository;
using ScimAPI.Utilities;
using System.Text.RegularExpressions;
using System.Collections;

namespace ScimLibrary.Services
{
    public class ScimUserService
    {
        IScimRepository<ScimUser> repository;
        public ScimUserService(IScimRepository<ScimUser> repository)
        {
            this.repository = repository;
        }

        public async Task<ScimUser> AddUser(ScimUser user)
        {
            await repository.AddAsync(user);
            user.Id = user.ExternalId;
            return user;
        }

        public void DeleteUser(ScimUser user)
        {
            repository.DeleteAsync(user.ExternalId);
        }

        public async Task<ScimUser> GetUserById(string externalId)
        {
            return await repository.GetByIdAsync(externalId);
        }

        public async Task<ScimListResponse<ScimUser>> GetUserByFilter(string filter)
        {
            var allUsers = await repository.GetAllAsync();

            var parsedFilter = ScimFilter.Parse(filter);

            var predicate = parsedFilter.ToLambda<ScimUser>();
            var matchedUsers = allUsers.Where(predicate).ToList();

            return new ScimListResponse<ScimUser>() { Resources = matchedUsers};
        }

        async Task ParseComplexPath(string path, string value2, ScimUser user)
        {
            string pattern = @"(\w+)\[(\w+)\s*(\S+)\s*""([^""]+)""\]";

            Match match = Regex.Match(path, pattern);
            if (match.Success)
            {
                string array = match.Groups[1].Value;
                string field = match.Groups[2].Value;
                string operatorType = match.Groups[3].Value;
                string value = match.Groups[4].Value;

                var property = typeof(ScimUser).GetProperty(array, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    var collection = property.GetValue(user) as IList; // Get the list property
                    if (collection != null)
                    {
                        Type itemType = property.PropertyType.GetGenericArguments()[0]; // Get the type of list items
                        object newItem = Activator.CreateInstance(itemType); // Create a new instance of list item type

                        PropertyInfo typeProp = itemType.GetProperty("Type");
                        PropertyInfo valueProp = itemType.GetProperty("Value");

                        if (typeProp != null) typeProp.SetValue(newItem, value); // Set "type" field
                        if (valueProp != null) valueProp.SetValue(newItem, value2); // Set "value" field

                        collection.Add(newItem); // Add to the list
                    }
                }

                await repository.UpdateAsync(user);
            }
        }

        public async Task<bool> PatchUserAsync(string userId, ScimPatchOperation patchOperations)
        {
            var user = await repository.GetByIdAsync(userId);
            if (user == null) return false;

            // Special clause for enterprise extension. Using reflections on extensions with nested value, would
            // ironically lead to messier code than just hardcoding them.

            string employeeNumberPropertyName = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber";
            string departmentPropertyName = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:department";
            string managerPropertyName = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:manager";


            foreach (var operation in patchOperations.Operations)
            {
                await ParseComplexPath(operation.Path,operation.Value.ToString(),user);
                var property = typeof(ScimUser).GetProperty(operation.Path, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if ((operation.Op == "Replace" || operation.Op == "Add") && operation.Path != null)
                {

                    if (string.Equals(operation.Path, employeeNumberPropertyName, StringComparison.OrdinalIgnoreCase)) {
                        user.EnterpriseExtension.EmployeeNumber = operation.Value.ToString();
                     }

                    if (string.Equals(operation.Path, departmentPropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        user.EnterpriseExtension.Department = operation.Value.ToString();
                    }

                    if (string.Equals(operation.Path, managerPropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        user.EnterpriseExtension.Manager.Value = operation.Value.ToString();
                    }

                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(user, Convert.ChangeType(operation.Value.ToString(), property.PropertyType));
                    }
                }
                else if (operation.Op == "Remove" && operation.Path != null)
                {

                    if (string.Equals(operation.Path, employeeNumberPropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        user.EnterpriseExtension.EmployeeNumber = null;
                    }

                    if (string.Equals(operation.Path, departmentPropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        user.EnterpriseExtension.Department = null;
                    }

                    if (string.Equals(operation.Path, managerPropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        user.EnterpriseExtension.Manager = null;
                    }

                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(user, null);
                    }
                }
            }

            await repository.UpdateAsync(user);
            return true;
        }

        public void UpdateUser(ScimUser user)
        {
            repository.UpdateAsync(user);
        }
    }
}
