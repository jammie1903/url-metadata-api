namespace UrlMetadata.Dtos
{
    public class ErrorResponseDto
    {
        public string Error { get; set; }

        public ErrorResponseDto(string error)
        {
            Error = error;
        }
    }
}
