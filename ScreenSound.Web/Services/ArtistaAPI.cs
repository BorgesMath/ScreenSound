using ScreenSound.Web.Requests;
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

    public async Task AddArtistaAsync(ArtistaRequest request)
    {
        await _httpClient.PostAsJsonAsync("artistas", request);
    }

    public async Task DeleteArtistaAsync(int Id)
    {


        await _httpClient.DeleteAsync($"artistas/{Id}");
    }

    public async Task<ArtistaResponse?> GetArtistaPorNomeAsync(string nome)
    {
        return await _httpClient.GetFromJsonAsync<ArtistaResponse>($"artistas/{nome}");
    }

    public async Task AtualizaArtistaAsync(ArtistaRequestEdit requestEdit)
    {
       await _httpClient.PutAsJsonAsync("artistas", requestEdit);
    }

    public async Task AvaliaArtistaAsync(int artistaId, double nota)
    {
        await _httpClient.PostAsJsonAsync("artistas/avaliacao", new { artistaId, nota });
    }

    public async Task<AvaliacaoDoArtistaResponse?> GetAvaliacaoDaPessoaLogadaAsync(int artistaId)
    {
        return await _httpClient
            .GetFromJsonAsync<AvaliacaoDoArtistaResponse?>($"artistas/{artistaId}/avaliacao");
    }

}
