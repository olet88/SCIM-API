using System.Text.Json.Serialization;

namespace ScimLibrary.BusinessModels
{
    public class Name
    {
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? Formatted { get; set; }
    }
}
