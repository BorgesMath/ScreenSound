using ScreenSound.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;
namespace ScreenSound.Web.Services;

public class ArtistaAPI(IHttpClientFactory factory)
{
    private readonly HttpClient _httpClient = factory.CreateClient("API");

    // Para listar os artistas do banco de dados:
    public async Task<ICollection<ArtistaResponse>?> GetArtistasAsync()
    {

        return await
            _httpClient.GetFromJsonAsync<ICollection<ArtistaResponse>>
            ("Artistas");
    }




}
