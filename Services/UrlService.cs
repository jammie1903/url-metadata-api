using System;
using System.Net;
using System.Text;
using UrlMetadata.Services.Interfaces;

namespace UrlMetadata.Services
{
    public class UrlService : IUrlService
    {
        public string ReadHeader(string url, int timeout)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);

            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.Accept = "*/*";
            request.Referer = url;
            request.UserAgent = "UrlMetadataApi";
            request.Headers.Add("cache-control", "no-cache");

            using (var response = (HttpWebResponse) request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                {
                    throw new Exception("the url did not return a response");
                }
                const int bytesToRead = 8092;
                var buffer = new byte[bytesToRead];
                var contents = "";
                int length;
                var headerStartPosition = -1;
                while ((length = stream.Read(buffer, 0, bytesToRead)) > 0)
                {
                    contents += Encoding.UTF8.GetString(buffer, 0, length);

                    if (headerStartPosition == -1)
                    {
                        var headerStart = contents.IndexOf("<head>", StringComparison.Ordinal);
                        if (headerStart != -1)
                        {
                            headerStartPosition = headerStart;
                        }
                    }

                    if (headerStartPosition != -1)
                    {
                        var headerEnd = contents.IndexOf("</head>", headerStartPosition, StringComparison.Ordinal);
                        if (headerEnd != -1)
                        {
                            var headerEndPosition = headerEnd + 7;
                            return "<html>" + contents.Substring(headerStartPosition, headerEndPosition - headerStartPosition) + "</html>";
                        }
                    }
                }
                return contents;
            }
        }
    }


}
