using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.Modelos;

public class Musica(string nome)
{
    public string Nome { get; set; } = nome;
    public int Id { get; set; }
    public int? AnoLancamento { get; set; }
    public int ArtistaId { get; set; }
    public virtual Artista? Artista { get; set; }

    public virtual ICollection<Genero>? Generos { get; set; }

    // Adicionando os Generos que a musica vai ter

    public void ExibirFichaTecnica()
    {
        Console.WriteLine($"Nome: {Nome}");
      
    }

    public override string ToString()
    {
        return @$"Id: {Id}
        Nome: {Nome}";
    }
}