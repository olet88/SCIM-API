using Newtonsoft.Json;
using ScimLibrary.BusinessModels;
using System.Net;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace ScimLibrary.Models
{
    public class ScimUser
    {
        /// <summary>
        /// The SCIM schema URIs that define the structure of this resource.
        /// For a user, this is typically the core User schema.
        /// </summary>
        public List<string> Schemas { get; set; } = new List<string>
        {
            "urn:ietf:params:scim:schemas:core:2.0:User",
        "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User",
        };

        public string? Id { get; set; }
        public string? ExternalId { get; set; }
        public string? PreferredLanguage { get; set; }
        public bool Active { get; set; }
        public string? UserName { get; set; }
        public Name? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Title { get; set; }
        public List<Email>? Emails { get; set; }
        public List<Address>? Addresses { get; set; }
        public List<PhoneNumber>? PhoneNumbers { get; set; }
        [JsonProperty("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User")]
        [JsonPropertyName("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User")]
        public EnterpriseExtension? EnterpriseExtension {  get; set; } = new EnterpriseExtension();
    }

    public class EnterpriseExtension
    {
        public string? EmployeeNumber { get; set; }
         public string? Department { get; set; }
    }
}
