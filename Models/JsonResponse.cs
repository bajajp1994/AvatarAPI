using System.Text.Json.Serialization;

namespace AvatarAPI.Models
{
    public class JsonResponse
    {
        [JsonPropertyName("Id")]
        public long Id { get; set; } //fileSizeBytes { get; set; }
        [JsonPropertyName("Url")]
        public string Url { get; set; }
    }

    //public class SampleDogWoofJsonResponse
    //{
    //    [JsonPropertyName("fileSizeBytes")]
    //    public long fileSizeBytes { get; set; } 
    //    [JsonPropertyName("url")]
    //    public string url { get; set; }
    //}
}
