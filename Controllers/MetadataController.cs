using System;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using UrlMetadata.ExtensionMethods;
using UrlMetadata.Enums;
using Microsoft.Extensions.Logging;
using UrlMetadata.Attributes;
using UrlMetadata.Dtos;
using UrlMetadata.Services.Interfaces;
using UrlMetadata.Utilities;

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
            [FromQueryDescribed("timeout", TimeoutDescription)] int timeout = 2000)
        {
            HtmlDocument doc;
            try
            {
                doc = LoadDocument(url, timeout);
            }
            catch (ErrorResponseException e)
            {
                return e.ErrorResponse;
            }

            try
            {

                Enum.TryParse(preference, true, out MetadataType metadataType);
                return doc.ExtractPageMetadata(metadataType);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error on ExtractPageMetadata for url: {url}", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto("An unexpected error occured"));
            }
        }

        private HtmlDocument LoadDocument(string url, int timeout)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogInformation("Request received with no url provided");
                throw new ErrorResponseException(BadRequest("Please specify a url"));
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
                throw new ErrorResponseException(NotFound("Page metadata could not be read for this url"));
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc;
        }

        [HttpGet]
        [RouteDescribed("metadata/all", "Gets all meta and link entries found in the header of the given url")]
        public ActionResult<RawMetadataDto> GetAllMetadata(
            [FromQueryDescribed("url", "The url you wish to lookup")] string url,
            [FromQueryDescribed("timeout", TimeoutDescription)] int timeout = 1000)
        {
            HtmlDocument doc;
            try
            {
                doc = LoadDocument(url, timeout);
            }
            catch (ErrorResponseException e)
            {
                return e.ErrorResponse;
            }

            try
            {
                return doc.GetAllMetadata();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error on GetAllMetadata for url: {url}", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto("An unexpected error occured"));
            }
        }

        [HttpGet]
        [RouteDescribed("metadata/tree", "Gets all meta/link entries and build a tree structure from them")]
        public ActionResult<TreeMetadataDto> GetMetadataTree(
            [FromQueryDescribed("url", "The url you wish to lookup")] string url,
            [FromQueryDescribed("timeout", TimeoutDescription)] int timeout = 1000)
        {
            HtmlDocument doc;
            try
            {
                doc = LoadDocument(url, timeout);
            }
            catch (ErrorResponseException e)
            {
                return e.ErrorResponse;
            }

            try
            {
                return doc.GetMetadataTree();
            }
            catch (Exception e)
            {
                _logger.LogError($"Error on GetMetadataTree for url: {url}", e);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto("An unexpected error occured"));
            }
        }
    }
}
