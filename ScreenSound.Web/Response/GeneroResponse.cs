namespace ScreenSound.Web.Response;

public record GeneroResponse(string Nome, string Descricao)
{
    public override string ToString()
    {
        return $"{this.Nome}";
    }
}
