using ScreenSound.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace ScreenSound.Web.Services;

public class AuthAPI(IHttpClientFactory factory)
{
    private readonly HttpClient _httpClient = factory.CreateClient("API");



    public async Task<(bool Success, string ErrorMessage, string SentJson)> LoginAsync(string email, string password)
    {
        try
        {
            var loginData = new
            {
                email,
                password
            };

            string sentJson = JsonSerializer.Serialize(loginData);

            var response = await _httpClient.PostAsJsonAsync("auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty, sentJson); // Sucesso
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"Código de status: {response.StatusCode}";
                }
                return (false, errorMessage, sentJson);
            }
        }
        catch (Exception ex)
        {
            return (false, $"Exceção: {ex.Message}", string.Empty);
        }
    }




}












