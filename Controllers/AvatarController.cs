using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Linq;
using System;
using Dapper;
using AvatarAPI.Models;
using System.Text.Json;


namespace AvatarAPI.Controllers
{
    [ApiController]
    [Route("avatar")]
    public class AvatarController : ControllerBase
    {
        // Static reference to the generated context
        private static readonly JsonResponseContext _context = new JsonResponseContext();

        private readonly HttpClient _httpClient;

        // Constructor to inject HttpClient
        public AvatarController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Endpoint to get avatar based on user identifier
        [HttpGet]
        public async Task<IActionResult> GetAvatar([FromQuery] string userIdentifier)
        {
            if (string.IsNullOrWhiteSpace(userIdentifier))
                return BadRequest("User identifier is required.");

            string imageUrl = null;

            // Rule 1: Last character is [6, 7, 8, 9]
            char lastChar = userIdentifier[^1];
            Console.WriteLine($"Last character: {lastChar}");

            if (char.IsDigit(lastChar) && "6789".Contains(lastChar))
            { 
                try
                {
                    // Fetch image URL from external source
                      string url = $"https://my-jsonserver.typicode.com/ck-pacificdev/tech-test/images/{lastChar}";
                   // string url = "https://random.dog/woof.json";
                    var response = await _httpClient.GetAsync(url);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var jsonResponse = JsonSerializer.Deserialize(responseContent, JsonResponseContext.Default.JsonResponse);
                        if (jsonResponse != null && !string.IsNullOrWhiteSpace(jsonResponse.Url))
                        {
                            return Ok(jsonResponse.Url);
                        }
                    }
                    return StatusCode((int)response.StatusCode, "Failed to fetch image from external service.");
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Error fetching image: {ex.Message}");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An unexpected error occurred. {ex.Message}");
                }
            }

            // Rule 2: Last character is [1, 2, 3, 4, 5]
            if (char.IsDigit(lastChar) && "12345".Contains(lastChar))
            {
                string databasePath = "data.db";
                string connectionString = $"Data Source={databasePath};";

                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync();


                string query = "SELECT url FROM images WHERE id = @Id";
                int id = (int)char.GetNumericValue(lastChar);

                var parameters = new Dictionary<string, object>{
                    { "Id", id }
                };

                var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);
                
                if (!string.IsNullOrEmpty(result))
                {
                    return Ok(result);
                }
            }

            // Rule 3: Contains at least one vowel
            if ("aeiouAEIOU".Any(userIdentifier.Contains))
            {
                imageUrl = "https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150";
                return Ok(imageUrl);
            }

            // Rule 4: Contains a non-alphanumeric character
            if (userIdentifier.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                var random = new Random();
                int randomNumber = random.Next(1, 6);
                imageUrl = $"https://api.dicebear.com/8.x/pixel-art/png?seed={randomNumber}&size=150";
                return Ok(imageUrl);
            }

            // Rule 5: Default case
            imageUrl = "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150";
            return Ok(imageUrl);
        }
    }
}

