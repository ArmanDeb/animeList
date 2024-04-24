using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace AnimeServices.Services
{
    public class AnimeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private const string FavoriteAnimeKey = "favoriteAnimeIds";

        public AnimeService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        // Assuming an endpoint URL for listing anime
        private string listAnimeEndpoint = "https://api.jikan.moe/v4/top/anime";
        // Endpoint for searching anime
        private string searchAnimeEndpoint = "https://api.jikan.moe/v4/anime";

        public async Task<List<AnimeData>> GetAnimeListAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<AnimeResponse>(listAnimeEndpoint);
                return response?.Data ?? new List<AnimeData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching anime list: {ex.Message}");
                return new List<AnimeData>();
            }
        }

        // New search method
        public virtual async Task<List<AnimeData>> SearchAnimeAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<AnimeResponse>($"{searchAnimeEndpoint}?q={Uri.EscapeDataString(query)}");
                return response?.Data ?? new List<AnimeData>();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching for anime: {ex.Message}");
                return new List<AnimeData>();
            }
        }

        public async Task ToggleFavoriteAsync(int malId)
        {
            var favorites = await GetFavoritesAsync();
            if (favorites.Contains(malId))
            {
                favorites.Remove(malId);
            }
            else
            {
                favorites.Add(malId);
            }
            await _localStorage.SetItemAsync(FavoriteAnimeKey, favorites);
        }

        public async Task<HashSet<int>> GetFavoritesAsync()
        {
            return await _localStorage.GetItemAsync<HashSet<int>>(FavoriteAnimeKey) ?? new HashSet<int>();
        }

        private async Task UpdatePersonalNoteAsync(AnimeData anime)
{
    // Save the updated personal note using localStorage or any other storage mechanism
    // For example, using localStorage:
    await _localStorage.SetItemAsync($"PersonalNote_{anime.Mal_id}", anime.PersonalNote);
}




       public async Task<List<AnimeData>> GetAnimesByIdsAsync(IEnumerable<int> animeIds)
{
    var animes = new List<AnimeData>();
    foreach (var id in animeIds)
    {
        var endpoint = $"https://api.jikan.moe/v4/anime/{id}";
        try
        {
            var animeResponse = await _httpClient.GetFromJsonAsync<SingleAnimeResponse>(endpoint);
            if (animeResponse?.Data != null)
            {
                animes.Add(animeResponse.Data); // Directly add the single object to the list
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching anime with ID {id}: {ex.Message}");
        }
    }
    return animes;
}

public class SingleAnimeResponse
{
    public AnimeData Data { get; set; } // Adjusted to correctly match a single AnimeData object response
}
    }

    public class AnimeResponse
    {
        public List<AnimeData> Data { get; set; }
    }

    public class AnimeData
    {
        public int Mal_id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; } // Ensure you have a Title property as it's crucial for search results.
        public string Synopsis { get; set; }
         public bool ShowSynopsis { get; set; }
        public AnimeImages Images { get; set; }
        public string PersonalNote { get; set; }
        // Other properties as needed
    }

    public class AnimeImages
    {
        public AnimeImageType Jpg { get; set; }
        // Include Webp if needed
    }

    public class AnimeImageType
    {
        public string Image_url { get; set; }
        public string Small_image_url { get; set; }
        public string Large_image_url { get; set; }
    }
}
