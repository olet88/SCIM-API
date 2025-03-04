using System.Text.Json.Nodes;

namespace ScimAPI.Utilities
{
    public class ScimPatchOperation
    {
        public required List<string> Schemas { get; set; }
        public required List<PatchOperation> Operations { get; set; }
    }

    public class PatchOperation
    {
        public required string Op { get; set; }
        public required string Path { get; set; }
        public required object Value { get; set; }
    }
}
