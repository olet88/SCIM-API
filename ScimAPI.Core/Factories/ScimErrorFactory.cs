using ScimLibrary.BusinessModels;

namespace ScimLibrary.Factories
{
    public interface IScimErrorFactory
    {
        ScimErrorResponse CreateUserNotFoundError(string id);
        ScimErrorResponse CreateUserAlreadyExistError(string userName);
        ScimErrorResponse CreateGroupNotFoundError(string id);
        ScimErrorResponse CreateUserGroupAlreadyExistError(string userGroupName);
    }

    public class ScimErrorFactory : IScimErrorFactory
    {
        public ScimErrorResponse CreateUserNotFoundError(string id)
        {
            return new ScimErrorResponse
            {
                Schemas = new List<string> { "urn:ietf:params:scim:api:messages:2.0:Error" },
                Detail = "User not found",
                Status = "404",
                Id = "404"
            };
        }

        public ScimErrorResponse CreateUserAlreadyExistError(string userName)
        {
            return new ScimErrorResponse
            {
                Schemas = new List<string> { "urn:ietf:params:scim:api:messages:2.0:Error" },
                Detail = "User already exist",
                Status = "409",
                Id = "409"
            };
        }

        public ScimErrorResponse CreateGroupNotFoundError(string id)
        {
            return new ScimErrorResponse
            {
                Schemas = new List<string> { "urn:ietf:params:scim:api:messages:2.0:Error" },
                Detail = "Group not found",
                Status = "404",
                Id = "404"
            };
        }

        public ScimErrorResponse CreateUserGroupAlreadyExistError(string userGroupName)
        {
            return new ScimErrorResponse
            {
                Schemas = new List<string> { "urn:ietf:params:scim:api:messages:2.0:Error" },
                Detail = "User group already exist",
                Status = "409",
                Id = "409"
            };
        }
    }
}
