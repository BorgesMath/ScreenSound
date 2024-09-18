namespace ScreenSound.API.Response;
public record ArtistaResponse(int Id, string Nome, string Bio, string? FotoPerfil)
{
    public double? Cassificacao { get; set; }



}
