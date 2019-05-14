using System;
using Microsoft.AspNetCore.Mvc;
using UrlMetadata.Dtos;

namespace UrlMetadata.Utilities
{
    public class ErrorResponseException : Exception
    {
        public ErrorResponseException(ObjectResult errorResponse)
        {
            ErrorResponse = errorResponse;
            if (ErrorResponse.Value is string)
                ErrorResponse.Value = new ErrorResponseDto((string)ErrorResponse.Value);
        }

        public ObjectResult ErrorResponse { get; set; }
    }
}
