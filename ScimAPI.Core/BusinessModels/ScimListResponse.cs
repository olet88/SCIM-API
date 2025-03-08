namespace ScimLibrary.BusinessModels
{
    public class ScimListResponse<T>
    {
        public int TotalResults { get; set; }
        public int ItemsPerPage { get; set; }
        public int StartIndex { get; set; }
        public List<string> Schemas { get; set; } = new List<string> { "urn:ietf:params:scim:schemas:core:2.0:ListResponse" };
        public List<T> Resources { get; set; } = new List<T>();
    }
}
