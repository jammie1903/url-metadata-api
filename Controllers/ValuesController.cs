using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using ExtensionMethods;

namespace url_preview.Controllers
{

    public class UriParams
    {
        public string uri;
    }


    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values/5
        [HttpGet]
        public ActionResult<object> Get([FromQuery(Name = "uri")] string uri)
        {
            string html;
            try
            {
                html = this.readHeader(uri);
            }
            catch (Exception e)
            {
                return NotFound("Header could not be read for this url: " + e.Message);
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.extractPageMetadata();
        }

        private string readHeader(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                int bytesToRead = 8092;
                byte[] buffer = new byte[bytesToRead];
                string contents = "";
                int length = 0;
                int headerStartPosition = -1;
                while ((length = stream.Read(buffer, 0, bytesToRead)) > 0)
                {
                    contents += Encoding.UTF8.GetString(buffer, 0, length);

                    if (headerStartPosition == -1)
                    {
                        var headerStart = contents.IndexOf("<head>");
                        if (headerStart != -1)
                        {
                            headerStartPosition = headerStart;
                        }
                    }

                    if (headerStartPosition != -1)
                    {
                        var headerEnd = contents.IndexOf("</head>", headerStartPosition);
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
