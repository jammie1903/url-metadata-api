using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;
using Enums;

namespace ExtensionMethods
{
    public static class ListExtensions
    {
        public static bool AddIfNotNull<T>(this List<T> list, T item)
        {
            if (item != null)
            {
                list.Add(item);
                return true;
            }
            return false;
        }
    }

    public static class HtmlDocumentExtensions
    {
        public static HtmlNode ReadFirstNode(this HtmlDocument document, string xpathLookup)
        {
            var nodes = document.DocumentNode.SelectNodes(xpathLookup);
            if (nodes == null)
            {
                return null;
            }
            return nodes.First();
        }

        public static string ReadFirstNodeValue(this HtmlDocument document, string xpathLookup)
        {
            var node = document.ReadFirstNode(xpathLookup);
            if (node == null)
            {
                return null;
            }
            return node.InnerText;
        }

        public static string ReadFirstNodeAttributeValue(this HtmlDocument document, string xpathLookup, string attributeName)
        {
            var node = document.ReadFirstNode(xpathLookup);
            if (node == null)
            {
                return null;
            }
            return node.GetAttributeValue(attributeName, null);
        }

        private static string[] Prioritise(MetadataType priority, string og, string twitter, string generic = null)
        {
            List<string> list = new List<string>();
            if (priority == MetadataType.og)
            {
                list.AddIfNotNull(og);
                list.AddIfNotNull(twitter);
                list.AddIfNotNull(generic);
            }
            else if (priority == MetadataType.twitter)
            {
                list.AddIfNotNull(twitter);
                list.AddIfNotNull(og);
                list.AddIfNotNull(generic);
            }
            else
            {
                list.AddIfNotNull(generic);
                list.AddIfNotNull(og);
                list.AddIfNotNull(twitter);
            }
            return list.ToArray();
        }

        private static string FirstNonEmptyString(params string[] values)
        {
            return values.FirstOrDefault(x => !string.IsNullOrEmpty(x));
        }

        public static object extractPageMetadata(this HtmlDocument document, MetadataType priority = MetadataType.og)
        {

            // http://www.iacquire.com/blog/18-meta-tags-every-webpage-should-have-in-2013

            // title
            // <title> tag
            // opengraph : <meta property=”og:title” content="thetitle"/>
            // twitter : <meta name=”twitter:title” content=”content”>
            var titles = new
            {
                generic = document.ReadFirstNodeValue("//head/title"),
                og = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:title']", "content"),
                twitter = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:title']", "content")
            };
            string title = FirstNonEmptyString(Prioritise(priority, titles.og, titles.twitter, titles.generic));

            // site name
            // <meta property="og:site_name" content="Stack Overflow" />
            string siteName = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:site_name']", "content");

            // domain
            // <meta name="twitter:domain" content="stackoverflow.com"/>
            string domain = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:domain']", "content");

            // description 
            // <meta name="description">
            // <meta property=”og:description” content="thedescription">
            // <meta name=”twitter:description” content=”the description”>
            var descriptions = new
            {
                generic = document.ReadFirstNodeAttributeValue("//head/meta[translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='description']", "content"),
                og = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:description']", "content"),
                twitter = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:description']", "content")
            };
            string description = FirstNonEmptyString(Prioritise(priority, descriptions.og, descriptions.twitter, descriptions.generic));

            // author
            // <link rel="author" href="authorUrl">
            string author = document.ReadFirstNodeAttributeValue("//head/link[@rel='author']", "href");

            // publisher - like author, but usally the business
            // <link rel=”publisher” href="authorUrl">
            string publisher = document.ReadFirstNodeAttributeValue("//head/link[@rel='publisher']", "href");

            // type
            // <meta property=”og:type” content=”article”/>
            string type = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:type']", "content");

            // card
            // <meta name=”twitter:card” content=”summary”>
            string card = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:card']", "content");

            // image
            // <meta property=”og:image” content=”http://www.iacquire.com/some-thumbnail.jpg”/>
            // <meta name=”twitter:image” content=”http://graphics8.nytimes.com/images/2012/02/19/us/19whitney-span/19whitney-span-articleLarge.jpg”>
            var images = new
            {
                og = document.ReadFirstNodeAttributeValue("//head/meta[@property='og:image']", "content"),
                twitter = document.ReadFirstNodeAttributeValue("//head/meta[@name='twitter:image']", "content")
            };
            string image = FirstNonEmptyString(Prioritise(priority, images.og, images.twitter));

            string favicon = document.ReadFirstNodeAttributeValue("//head/link[contains(@rel,'icon')]", "href");

            // additional data
            //<meta name="twitter:label1" value="Opens in Theaters" />
            //<meta name="twitter:data1" value="December 1, 2015" />
            List<object> additionalInformation = new List<object>();
            for (int count = 1; count <= 10; count++)
            {
                string label = document.ReadFirstNodeAttributeValue($"//head/meta[@name='twitter:label{count}']", "value");
                string data = document.ReadFirstNodeAttributeValue($"//head/meta[@name='twitter:data{count}']", "value");
                if (!string.IsNullOrEmpty(label) || !string.IsNullOrEmpty(data))
                {
                    additionalInformation.Add(new { label, data });
                }
                else
                {
                    break;
                }
            }

            return new { title, titles, domain, description, descriptions, author, publisher, type, card, image, images, favicon, additionalInformation = additionalInformation.ToArray() };
        }
    }
}