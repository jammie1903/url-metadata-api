using System;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Cors;
using UrlMetadata.ExtensionMethods;
using UrlMetadata.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using UrlMetadata.Attributes;
using UrlMetadata.Dtos;
using UrlMetadata.Services.Interfaces;

namespace UrlMetadata.Controllers
{
    [Route("/api")]
    [ApiController]
    [EnableCors("_AllowAllOrigins")]
    public class MetadataController : ControllerBase
    {
        private const string PreferenceDescription = @"Which type of metadata should be treated as priority, either
        <a target=""_blank"" rel=""noopener"" href=""http://ogp.me/"">OpenGraph</a> (the default),
        <a target = ""_blank"" rel=""noopener"" href=""https://developer.twitter.com/en/docs/tweets/optimize-with-cards/guides/getting-started.html"">Twitter</a>, or generic.";

        private const string AllDescription =
            "Set this to <code>true</code> if you wish to retrieve all types of metadata for those that can be provided in multiple formats. eg. titles, descriptions and images.";

        private const string TimeoutDescription =
            "How long the api should wait for a response from the given url. Has an allowed range of 100 to 3000 milliseconds.";

        private readonly ILogger<MetadataController> _logger;
        private readonly IUrlService _urlService;

        public MetadataController(ILogger<MetadataController> logger,
            IUrlService urlService)
        {
            _logger = logger;
            _urlService = urlService;
        }

        [HttpGet]
        [RouteDescribed("ping", "A basic ping endpoint you can use to make sure that the service is still running")]
        public ActionResult<PingResponseDto> Ping()
        {
            return new PingResponseDto { DateTime = DateTime.Now};
        }

        [HttpGet]
        [RouteDescribed("metadata", "The main endpoint of this api, used to get the metadata of a given url")]
        public ActionResult<UrlMetadataDto> GetMetadata(
            [FromQueryDescribed("url", "The url you wish to lookup")] string url,
            [FromQueryDescribed("preference", PreferenceDescription)] string preference = "OpenGraph",
            [FromQueryDescribed("all", AllDescription)] bool all = false,
            [FromQueryDescribed("timeout", TimeoutDescription)] int timeout = 1000)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogInformation("Request received with no url provided");
                return BadRequest(new ErrorResponseDto("Please specify a url"));
            }

            timeout = Math.Max(Math.Min(timeout, 3000), 100);

            string html;
            try
            {
                html = _urlService.ReadHeader(url, timeout);
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Could not read url: {url}", e);
                return NotFound(new ErrorResponseDto("Page metadata could not be read for this url"));
            }

            Enum.TryParse(preference, true, out MetadataType metadataType);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.ExtractPageMetadata(metadataType, all);
        }
    }
}
