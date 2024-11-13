using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PacificProgrammingWebApi.Model;
using System;

namespace PacificProgrammingWebApi.Services
{

    public class ClientService
    {
        private readonly HttpClient _httpClient;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public ClientService(HttpClient httpClient, DataContext context, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _context = context;
            _configuration = configuration;
        }

        public ClientService()
        {
        }

        public async Task<string> GetImageUrlAsync(string userIdentifier)
        {
            var lastDigit = userIdentifier[^1].ToString();  

            // Case 1: If the last character is [6, 7, 8, 9]
            if ("6789".Contains(lastDigit))
            {
                var url = await GetExternalImageUrlAsync(lastDigit);
                if (url != null) return url;
            }

            // Case 2: If the last character is [1, 2, 3, 4, 5], query from SQLite database
            if ("12345".Contains(lastDigit))
            {
                var url = await GetImageUrlFromDatabaseAsync(lastDigit);
                if (url != null) return url;
            }

            // Case 3: If the identifier contains any vowel characters
            if (userIdentifier.Any(c => "aeiou".Contains(c.ToString().ToLower())))
            {
                return GetRandomImageUrl("vowel");
            }

            // Case 4: If the identifier contains a non-alphanumeric character
            if (userIdentifier.Any(c => !char.IsLetterOrDigit(c)))
            {
                var randomSeed = new Random().Next(1, 6);
                return GetRandomImageUrl(randomSeed.ToString());
            }

            // Case 5: If none of the above, return the default image
            return GetRandomImageUrl("default");
        }

        // get image URL from external API - for last digits 6-9
        private async Task<string> GetExternalImageUrlAsync(string lastDigit)
        {
            var url = $"https://my-json-server.typicode.com/ck-pacificdev/tech-test/images/{lastDigit}";
            var response = await _httpClient.GetStringAsync(url);
            var imageResponse = JsonConvert.DeserializeObject<images>(response);
            return imageResponse?.url;
        }

        // get image from SQLite database 
        private async Task<string> GetImageUrlFromDatabaseAsync(string lastDigit)
        {
            int id = int.Parse(lastDigit);
            var image = await _context.images.FirstOrDefaultAsync(img => img.id == id);
            return image?.url;
        }

        //get other case image URL
        private string GetRandomImageUrl(string seed)
        {
            return $"https://api.dicebear.com/8.x/pixel-art/png?seed={seed}&size=150";
        }
    }

}


