using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UrlMetadata.Dtos;

namespace UrlMetadata.Middleware
{
    public class CustomJsonOutputFormatter : JsonOutputFormatter
    {
        public CustomJsonOutputFormatter() : base(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        }, ArrayPool<char>.Shared)
        {
        }

        /// <inheritdoc />
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (selectedEncoding == null)
                throw new ArgumentNullException(nameof(selectedEncoding));
            using (var writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                WriteObject(writer, context.Object is ErrorResponseDto 
                    ? context.Object : new {data = context.Object});

                await writer.FlushAsync();
            }
        }
    }
}
