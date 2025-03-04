using System.Text.Json.Serialization;

namespace ScimLibrary.BusinessModels
{
    public class ScimErrorResponse
    {
        public required List<string> Schemas { get; set; }
        public required string Detail { get; set; }
        public required string Status { get; set; }
        public required string Id { get; set; }
    }
}
