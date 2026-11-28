using System.Text.Json.Serialization;

namespace Core.Features
{
    public class MainResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public object? Content { get; set; }

        [JsonIgnore]
        public int StatusCode { get; set; } = 200;

        public static MainResponse Ok(object? content, int statusCode = 200) =>
            new() { IsSuccess = true, ErrorMessage = string.Empty, Content = content, StatusCode = statusCode };

        public static MainResponse Fail(int statusCode, string message) =>
            new() { IsSuccess = false, ErrorMessage = message, StatusCode = statusCode };
    }
}
