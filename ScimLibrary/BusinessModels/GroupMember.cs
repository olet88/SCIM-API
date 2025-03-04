using System.Text.Json.Serialization;

namespace ScimLibrary.BusinessModels
{
    public class GroupMember
    {
        public string? Value { get; set; }
        public string? Ref { get; set; }
        public string? Display { get; set; }
    }
}
