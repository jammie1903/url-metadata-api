using Microsoft.AspNetCore.Mvc;

namespace UrlMetadata.Attributes
{
    public interface IDescribed
    {
        string Description { get; }
    }
}
