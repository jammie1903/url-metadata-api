namespace UrlMetadata.Dtos.ApiDoc
{
    public class QueryParameterDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public string Type { get; set; }
        public bool Mandatory { get; set; }
    }
}
