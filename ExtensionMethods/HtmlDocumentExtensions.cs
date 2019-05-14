using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;
using UrlMetadata.Dtos;
using UrlMetadata.Enums;
using UrlMetadata.Utilities;

namespace UrlMetadata.ExtensionMethods
{
    public static class HtmlDocumentExtensions
    {
        public static HtmlNode ReadFirstNode(this HtmlDocument document, string xpathLookup)
        {
            var nodes = document.DocumentNode.SelectNodes(xpathLookup);
            return nodes?.First();
        }

        public static IEnumerable<HtmlNode> ReadNodes(this HtmlDocument document, string xpathLookup)
        {
            return document.DocumentNode.SelectNodes(xpathLookup)?.Elements() ?? new HtmlNode[0];
        }

        public static string ReadFirstNodeValue(this HtmlDocument document, string xpathLookup)
        {
            var node = document.ReadFirstNode(xpathLookup);
            return node?.InnerText;
        }

        public static string ReadFirstNodeAttributeValue(this HtmlDocument document, string xpathLookup, string attributeName)
        {
            var node = document.ReadFirstNode(xpathLookup);
            return node?.GetAttributeValue(attributeName, null);
        }

        private static string Prioritise(MetadataType priority, Alternatives alternatives)
        {
            switch (priority)
            {
                case MetadataType.OpenGraph:
                    return alternatives.OpenGraph
                        .Or(alternatives.Twitter)
                        .Or(alternatives.Generic);
                case MetadataType.Twitter:
                    return alternatives.Twitter
                        .Or(alternatives.OpenGraph)
                        .Or(alternatives.Generic);
                default:
                    return alternatives.Generic
                        .Or(alternatives.OpenGraph)
                        .Or(alternatives.Twitter);
            }
        }

        public static RawMetadataDto GetAllMetadata(this HtmlDocument document)
        {
            var title = document.ReadFirstNodeValue("//head/title");
            var meta = document.ReadNodes("//head/meta");

            var metaEntries = new Dictionary<string, string>();

            foreach (var htmlNode in meta)
            {
                var content = htmlNode.GetAttributeValue("content", null);
                if (string.IsNullOrEmpty(content)) continue;

                var name = htmlNode.GetAttributeValue("name", null);
                if (!string.IsNullOrEmpty(name)) metaEntries.ForceAdd(name, content);

                var property = htmlNode.GetAttributeValue("property", null);
                if (!string.IsNullOrEmpty(property)) metaEntries.ForceAdd(property, content);
            }

            var links = document.ReadNodes("//head/link");

            var linkEntries = new Dictionary<string, string>();

            foreach (var htmlNode in links)
            {
                var rel = htmlNode.GetAttributeValue("rel", null);
                if (string.IsNullOrEmpty(rel)) continue;

                var href = htmlNode.GetAttributeValue("href", null);
                if (!string.IsNullOrEmpty(href)) linkEntries.ForceAdd(rel, href);
            }

            return new RawMetadataDto { Title = title, Meta = metaEntries, Links = linkEntries };
        }

        public static TreeMetadataDto GetMetadataTree(this HtmlDocument document)
        {
            var title = document.ReadFirstNodeValue("//head/title");
            var meta = document.ReadNodes("//head/meta");

            var tree = new MetaTree();

            foreach (var htmlNode in meta)
            {
                var content = htmlNode.GetAttributeValue("content", null);
                if (string.IsNullOrEmpty(content)) continue;

                var name = htmlNode.GetAttributeValue("name", null);
                if (!string.IsNullOrEmpty(name)) tree.Add(name, content);

                var property = htmlNode.GetAttributeValue("property", null);
                if (!string.IsNullOrEmpty(property)) tree.Add(property, content);
            }

            var linkTree = new MetaTree();

            var links = document.ReadNodes("//head/link");
           
            foreach(var htmlNode in links)
            {
                var rel = htmlNode.GetAttributeValue("rel", null);
                if (string.IsNullOrEmpty(rel)) continue;

                var href = htmlNode.GetAttributeValue("href", null);
                if (!string.IsNullOrEmpty(href)) linkTree.Add(rel, href);
            }

            return new TreeMetadataDto {Title = title, Meta = tree.Root, Links = linkTree.Root };
        }

        /// <summary>
        /// Extracts information from the given documents metadata.
        /// <see href="http://www.iacquire.com/blog/18-meta-tags-every-webpage-should-have-in-2013">More information can be found here</see>
        /// </summary>
        /// <param name="document"></param>
        /// <param name="priority"></param>
        /// <param name="returnAll">if true, all alternatives are returned for fields with multiple sources</param>
        /// <returns></returns>
        public static UrlMetadataDto ExtractPageMetadata(this HtmlDocument document, MetadataType priority)
        {
            // title
            // <title> tag
            // opengraph : <meta property=”og:title” content="thetitle"/>
            // twitter : <meta name=”twitter:title” content=”content”>
            var titles = new Alternatives
            {
                Generic = document.ReadFirstNodeValue("//head/title"),
                OpenGraph = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:title']", "content"),
                Twitter = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:title']", "content")
            };
            var title = Prioritise(priority, titles);

            // site name
            // <meta property="og:site_name" content="Stack Overflow" />
            var siteName = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:site_name']", "content");

            // domain
            // <meta name="twitter:domain" content="stackoverflow.com"/>
            var domain = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:domain']", "content");

            // description 
            // <meta name="description">
            // <meta property=”og:description” content="thedescription">
            // <meta name=”twitter:description” content=”the description”>
            var descriptions = new Alternatives
            {
                Generic = document.ReadFirstNodeAttributeValue("//head/meta[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='description']", "content"),
                OpenGraph = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:description']", "content"),
                Twitter = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:description']", "content")
            };
            var description = Prioritise(priority, descriptions);

            // author
            // <meta name="author" content="John Smith">
            var author = document.ReadFirstNodeAttributeValue("//head/meta[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='author']", "content");

            // theme color
            // <meta name="theme-color" content="#048598" />
            var themeColor = document.ReadFirstNodeAttributeValue(
                "//head/meta[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='theme-color']",
                "content");

            // type
            // <meta property=”og:type” content=”article”/>
            var type = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:type']", "content");

            // card
            // <meta name=”twitter:card” content=”summary”>
            var card = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:card']", "content");

            // image
            // <meta property=”og:image” content=”http://www.iacquire.com/some-thumbnail.jpg”/>
            // <meta name=”twitter:image” content=”http://graphics8.nytimes.com/images/2012/02/19/us/19whitney-span/19whitney-span-articleLarge.jpg”>
            var images = new Alternatives
            {
                OpenGraph = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:image']", "content"),
                Twitter = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:image']", "content")
            };
            var image = Prioritise(priority, images);

            var favicon = document.ReadFirstNodeAttributeValue("//head/link[contains(@rel,'icon')]", "href");

            // additional data
            // <meta name="twitter:label1" value="Opens in Theaters" />
            // <meta name="twitter:data1" value="December 1, 2015" />
            var additionalInformation = new List<AdditionalInformationDto>();
            for (var count = 1; count <= 10; count++)
            {
                var label = document.ReadFirstNodeAttributeValue($"//head/meta[@name='twitter:label{count}']", "value");
                var data = document.ReadFirstNodeAttributeValue($"//head/meta[@name='twitter:data{count}']", "value");
                if (!string.IsNullOrEmpty(label) || !string.IsNullOrEmpty(data))
                {
                    additionalInformation.Add(new AdditionalInformationDto { Label = label, Data = data });
                }
                else
                {
                    break;
                }
            }

            return new UrlMetadataDto {
                Title = title,
                Domain = domain,
                SiteName = siteName,
                Description = description,
                Author = author,
                ThemeColor = themeColor,
                Type = type,
                Card = card,
                Image = image,
                Favicon = favicon,
                AdditionalInformation = additionalInformation
            };
        }
    }
}