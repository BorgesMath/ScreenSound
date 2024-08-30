namespace ScreenSound.Web.Response;

public record AuthResponse
{
    public bool Sucesso { get; set; }
    public string[] Erros { get; set; }
}
