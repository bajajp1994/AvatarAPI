using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AvatarAPI.Models
{
    // This class enables source generation for the `JsonResponse` type.
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization | JsonSourceGenerationMode.Metadata)]
    [JsonSerializable(typeof(JsonResponse))]
    public partial class JsonResponseContext : JsonSerializerContext { }

}
