using ScreenSound.Web.Response;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;

namespace ScreenSound.Web.Services;

public class AuthAPI(IHttpClientFactory factory)
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    public async Task<AuthResponse> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login", new
        {
            username,
            password
        });

        if (response.IsSuccessStatusCode)
        {
            return new AuthResponse { Sucesso = true };
        }

        return new AuthResponse { Sucesso = false, Erros = ["Login ou Senha Invalidos"] };

       

    }   

}
