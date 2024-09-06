using Microsoft.AspNetCore.Components.Authorization;
using ScreenSound.Web.Response;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ScreenSound.Web.Services;

public class AuthAPI(IHttpClientFactory factory) : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient = factory.CreateClient("API");

    private bool autenticado = false;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        autenticado = false;
        ClaimsPrincipal pessoa = new(); 
        // Sem info = não autenticado

        var resp = await _httpClient.GetAsync("auth/manage/info");
        if (resp.IsSuccessStatusCode)
        {
            var info = await resp.Content.ReadFromJsonAsync<InfoPessoaResponse>();
       

        if (info is not null)
        {
            Claim[] dados =
                [
                    new Claim(ClaimTypes.Name, info.Email), // Pega o dado de email e leva pro Name
                    new Claim(ClaimTypes.Email, info.Email), // O mesmo pro email
                ];

            var idenity = new ClaimsIdentity(dados, "Cookies"); // Constroi a Identidade baseado no nome e email
                                                                // com a autenticação via Cookies

            pessoa = new ClaimsPrincipal(idenity);
                // Construido info, autenticado


                autenticado = true;

            }
        }
        return new AuthenticationState(pessoa);
    }

    public async Task<AuthResponse> LoginAsync(string email, string senha)
    {
        try
        {

            var response = await _httpClient.PostAsJsonAsync("auth/login", new
            {
                email,
                password = senha
            });

            if (response.IsSuccessStatusCode)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return new AuthResponse { Sucesso = true };
            }

            return new AuthResponse { Sucesso = false, Erros = ["Login/senha inválidos"] };
        }
        catch (Exception ex)
        {
            return new AuthResponse { Sucesso = false, Erros = [ex.Message] };
        }
    }

    public async Task LogoutAsync()
    {
        await _httpClient.PostAsync("auth/logout", null);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }


    public async Task<bool> VerificaAutenticado()
    {
        await GetAuthenticationStateAsync();
        return autenticado;
    }
}