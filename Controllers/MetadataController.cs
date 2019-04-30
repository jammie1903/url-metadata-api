using System;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Cors;
using UrlMetadata.ExtensionMethods;
using UrlMetadata.Enums;
using Microsoft.Extensions.Logging;
using UrlMetadata.Services.Interfaces;

namespace UrlMetadata.Controllers
{
    [Route("/api")]
    [ApiController]
    [EnableCors("_AllowAllOrigins")]
    public class MetadataController : ControllerBase
    {
        private readonly ILogger<MetadataController> _logger;
        private readonly IUrlService _urlService;

        public MetadataController(ILogger<MetadataController> logger,
            IUrlService urlService)
        {
            _logger = logger;
            _urlService = urlService;
        }

        [HttpGet]
        [Route("ping")]
        public ActionResult<object> Ping()
        {
            return new { data = new {datetime = DateTime.Now} };
        }

        [HttpGet]
        [Route("metadata")]
        public ActionResult<object> Get(
            [FromQuery(Name = "url")] string url,
            [FromQuery(Name = "priority")] string priority,
            [FromQuery(Name = "all")] bool all,
            [FromQuery(Name = "timeout")] int timeout = 1000)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogInformation("Request received with no url provided");
                return BadRequest("Please specify a url");
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
                return NotFound("Page metadata could not be read for this url");
            }

            Enum.TryParse(priority, true, out MetadataType metadataType);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return new { data = doc.ExtractPageMetadata(metadataType, all) };
        }
    }
}
