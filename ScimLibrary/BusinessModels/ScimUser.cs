using Newtonsoft.Json;
using ScimAPI.BusinessModels;
using ScimLibrary.BusinessModels;
using System.Text.Json.Serialization;

namespace ScimLibrary.Models
{
    public class ScimUser
    {
        public List<string> Schemas { get; set; } = new List<string>
        {
            "urn:ietf:params:scim:schemas:core:2.0:User",
        "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User",
        };

        public string? Id { get; set; }
        public required string ExternalId { get; set; }
        public string? PreferredLanguage { get; set; }
        public bool Active { get; set; }
        public string? UserName { get; set; }
        public Name? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Title { get; set; }
        public List<Email>? Emails { get; set; } = new List<Email>();
        public List<Address>? Addresses { get; set; } = new List<Address>();
        public List<PhoneNumber>? PhoneNumbers { get; set; } = new List<PhoneNumber>();
        [JsonProperty("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User")]
        [JsonPropertyName("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User")]
        public EnterpriseExtension? EnterpriseExtension { get; set; } = new EnterpriseExtension();
    }

        public class EnterpriseExtension
    {
        public string? EmployeeNumber { get; set; }
        public string? Department { get; set; }
        public Manager Manager { get; set; } = new Manager();
    }
}
