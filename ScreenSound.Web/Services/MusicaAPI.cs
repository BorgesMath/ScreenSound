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

    public async Task<(bool Success, string ErrorMessage)> AddMusicaAsync(MusicaRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("musicas", request);

            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty); // Sucesso
            }
            else
            {
                // Captura o conteúdo da mensagem de erro, se houver
                var errorMessage = await response.Content.ReadAsStringAsync();

                // Se a mensagem de erro estiver vazia, fornece um fallback com o código de status
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"Código de status: {response.StatusCode}";
                }

                return (false, errorMessage);
            }
        }
        catch (Exception ex)
        {
            // Captura exceções que podem ocorrer durante a requisição
            return (false, $"Exceção: {ex.Message}");
        }
    }



}