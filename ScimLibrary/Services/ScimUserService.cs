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

            return new ScimListResponse<ScimUser>() { Resources = matchedUsers};
        }

        async Task ParseComplexPath(string path, string value2, ScimUser user)
        {
            string pattern = @"(\w+)\[(\w+)\s*(\S+)\s*""([^""]+)""\](?:\.(\w+))?";
            string toUpperCase = @"(?:^|[_\s-])([a-z])";

            Match match = Regex.Match(path, pattern);
            if (match.Success)
            {
                string array = match.Groups[1].Value;
                string field = match.Groups[2].Value;
                string operatorType = match.Groups[3].Value;
                string value = match.Groups[4].Value;
                string fieldToUpdate = match.Groups[5].Value;

                field = Regex.Replace(field, @"(?:^|[_\s-])([a-z])", match => match.Groups[1].Value.ToUpper());
                fieldToUpdate = Regex.Replace(fieldToUpdate, @"(?:^|[_\s-])([a-z])", match => match.Groups[1].Value.ToUpper());


                var property = typeof(ScimUser).GetProperty(array, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    var collection = property.GetValue(user) as IList; // Get the list property
                    if (collection != null)
                    {
                        Type itemType = property.PropertyType.GetGenericArguments()[0]; 
                        PropertyInfo typeProp = itemType.GetProperty(field); // convert these to pascal case
                        PropertyInfo valueProp = itemType.GetProperty(fieldToUpdate);

                        bool exists = collection.Cast<object>().Any(item => typeProp.GetValue(item)?.Equals(value) == true);
                        object patchedItem;

                        object newItem = Activator.CreateInstance(itemType);

                        if (!exists)
                        {
                            patchedItem = newItem;
                        } else
                        {
                            patchedItem = collection.Cast<object>().Where(item => typeProp.GetValue(item)?.Equals(value) == true).FirstOrDefault();
                        }

                        if (typeProp != null) typeProp.SetValue(patchedItem, value); // Set "type" field
                        if (valueProp != null) 
                            valueProp.SetValue(patchedItem, value2); // Set "value" field

                        if (!exists)
                            collection.Add(patchedItem); // Add to the list
                    }
                }

                await repository.UpdateUserAsync(user);
            }
        }

        public async Task<bool> PatchUserAsync(string userId, ScimPatchOperation patchOperations)
        {
            var user = await repository.GetUserByIdAsync(userId);
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

            await repository.UpdateUserAsync(user);
            return true;
        }

        public void UpdateUser(ScimUser user)
        {
            repository.UpdateUserAsync(user);
        }
    }
}
