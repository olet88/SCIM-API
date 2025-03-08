using ScimLibrary.Models;
using System.Reflection;
using ScimLibrary.BusinessModels;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using ScimAPI.Repository;
using ScimAPI.Utilities;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;

namespace ScimLibrary.Services
{
    public class ScimUserService
    {
        IScimUserRepository repository;
        public ScimUserService(IScimUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<ScimUser> AddUser(ScimUser user)
        {
            await repository.AddUserAsync(user);
            user.Id = user.ExternalId;
            return user;
        }

        public void DeleteUser(ScimUser user)
        {
            repository.DeleteUserAsync(user.ExternalId);
        }

        public async Task<ScimUser> GetUserById(string externalId)
        {
            return await repository.GetUserByIdAsync(externalId);
        }

        public async Task<ScimListResponse<ScimUser>> GetUserByFilter(string filter)
        {
            var allUsers = await repository.GetAllUsersAsync();

            var parsedFilter = ScimFilter.Parse(filter);

            var predicate = parsedFilter.ToLambda<ScimUser>();
            var matchedUsers = allUsers.Where(predicate).ToList();

            return new ScimListResponse<ScimUser>() { Resources = matchedUsers };
        }

        async Task ParseEnterpriseExtension(PatchOperation operation, ScimUser user)
        {
            // Special clause for enterprise extension. Using reflections on extensions with nested value, would
            // ironically lead to messier code than just hardcoding them.

            string employeeNumberPropertyName = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber";
            string departmentPropertyName = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:department";
            string managerPropertyName = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:manager";

            if ((operation.Op == "Replace" || operation.Op == "Add") && operation.Path != null)
            {

                if (string.Equals(operation.Path, employeeNumberPropertyName, StringComparison.OrdinalIgnoreCase))
                {
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
            }
        }

        async Task ParseComplexPath(string path, string valueOfField, ScimUser user)
        {
            string pattern = @"(\w+)\[(\w+)\s*(\S+)\s*""([^""]+)""\](?:\.(\w+))?";
            Match match = Regex.Match(path, pattern);

            if (!match.Success) return;

            string arrayName = match.Groups[1].Value;
            string field = ToPascalCase(match.Groups[2].Value);
            string type = match.Groups[4].Value;
            string fieldToUpdate = ToPascalCase(match.Groups[5].Value);

            var property = typeof(ScimUser).GetProperty(arrayName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null || !(property.GetValue(user) is IList collection)) return;

            Type itemType = property.PropertyType.GetGenericArguments().FirstOrDefault();
            if (itemType == null) return;

            PropertyInfo typeProp = itemType.GetProperty(field);
            PropertyInfo? valueProp = itemType.GetProperty(fieldToUpdate);

            if (typeProp == null || valueProp == null) return;

            object patchedItem = collection.Cast<object>().FirstOrDefault(item => typeProp.GetValue(item)?.Equals(type) == true) ?? Activator.CreateInstance(itemType);

            typeProp.SetValue(patchedItem, type);
            valueProp.SetValue(patchedItem, valueOfField);

            if (!collection.Contains(patchedItem))
            {
                collection.Add(patchedItem);
            }

            await repository.UpdateUserAsync(user);
        }

        private static string ToPascalCase(string input)
        {
            return Regex.Replace(input, @"(?:^|[_\s-])([a-z])", match => match.Groups[1].Value.ToUpper(CultureInfo.InvariantCulture));
        }

        public async Task<bool> PatchUserAsync(string userId, ScimPatchOperation patchOperations)
        {
            var user = await repository.GetUserByIdAsync(userId);
            if (user == null) return false;

            foreach (var operation in patchOperations.Operations)
            {
                if (operation.Path.Contains("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User"))
                {
                    await ParseEnterpriseExtension(operation, user);
                    return true;
                }
                else if (operation.Path.Contains("[type eq"))
                {
                }

                var property = typeof(ScimUser).GetProperty(operation.Path, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if ((operation.Op == "Replace" || operation.Op == "Add") && operation.Path != null)
                {
                    if (operation.Path.Contains("[type eq"))
                    {
                        await ParseComplexPath(operation.Path, operation.Value.ToString(), user);
                    }

                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(user, Convert.ChangeType(operation.Value.ToString(), property.PropertyType));
                    }
                }
                else if (operation.Op == "Remove" && operation.Path != null)
                {
                    if (operation.Path.Contains("[type eq"))
                    {
                        await ParseComplexPath(operation.Path, null, user);
                    }
                    if (property != null && property.CanWrite)
                    {
                        property.SetValue(user, null);
                    }
                }
            }

            await repository.UpdateUserAsync(user);
            return true;
        }

        public void UpdateUser(ScimUser user)
        {
            repository.UpdateUserAsync(user);
        }
    }
}
