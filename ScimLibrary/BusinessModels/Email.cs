using System.Text.Json.Serialization;

namespace ScimLibrary.BusinessModels
{
    public class Email
    {
        public string? Value { get; set; }
        public string? Type { get; set; }
        public bool Primary { get; set; }
    }
}
