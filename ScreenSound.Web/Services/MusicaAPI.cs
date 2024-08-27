using global::ScreenSound.Web.Response;
using ScreenSound.Web.Requests;
using System.Net.Http.Json;

namespace ScreenSound.Web.Services;

public class MusicaAPI(IHttpClientFactory factory)
{
    private readonly HttpClient _httpClient = factory.CreateClient("API");

    public async Task<ICollection<MusicaResponse>?> GetMusicasAsync()
    {
        return await _httpClient.GetFromJsonAsync<ICollection<MusicaResponse>>("musicas");
    }

    public async Task AddMusicaAsync(MusicaRequest request)
    {
        await _httpClient.PostAsJsonAsync("musicas", request);
    }

}