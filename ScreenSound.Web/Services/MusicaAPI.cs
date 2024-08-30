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
            var response = await _httpClient.PostAsJsonAsync("musica", request);

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

    public async Task<(bool Success, string ErrorMessage)> DeleteMusicaAsync(int Id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"musica/{Id}");

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


    public async Task<(bool Success, string ErrorMessage)> AtualizarMusicaAsync(MusicaRequestEdit requestEdit)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("musica", requestEdit);
            // Ver se é musicas ou musica

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

    public async Task<(MusicaResponse? response, bool Success, string ErrorMessage)> GetMusicaPorNomeAsync(string nome)
    {
        try
        {
            // Verifique se o endpoint é "musicas" ou "musica"
            var response = await _httpClient.GetFromJsonAsync<MusicaResponse>($"musicas/{nome}");

            if (response != null) // Corrigido para verificar se 'response' não é nulo
            {
                return (response, true, string.Empty); // Sucesso
            }
            else
            {
                // Como 'response' é nulo, não podemos acessar 'IsSuccessStatusCode'
                return (null, false, "Erro ao obter a música, resposta nula.");
            }
        }
        catch (HttpRequestException httpEx)
        {
            // Captura exceções relacionadas a problemas com a requisição HTTP
            return (null, false, $"Erro de requisição HTTP: {httpEx.Message}");
        }
        catch (Exception ex)
        {
            // Captura exceções gerais
            return (null, false, $"Exceção: {ex.Message}");
        }
    }



}