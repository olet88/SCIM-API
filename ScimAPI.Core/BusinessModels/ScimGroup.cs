namespace ScimLibrary.Models
{
    public class ScimGroup
    {
        public List<string>? Schemas { get; set; }
        public required string ExternalId { get; set; }
        public string? Id { get; set; }
        public required string DisplayName { get; set; }
        public IEnumerable<ScimMember>? Members { get; set; }
        public ScimMeta? Meta { get; set; }
    }

    public class ScimMember
    {
        public string Value { get; set; }
        public string? Ref { get; set; }
    }

    public class ScimMeta
    {
        public string? ResourceType { get; set; }
    }
}