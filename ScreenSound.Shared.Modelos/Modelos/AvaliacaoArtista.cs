namespace ScreenSound.Shared.Modelos.Modelos;

public class AvaliacaoArtista
{

    public int ArtistaID { get; set; }
    public virtual Artista? Artista { get; set; }
    // Virtual para que quando for necessario o Entity FreameWork carregue 
    // a info de artista
    public int PessoaID { get; set; }

    public int Nota { get; set; }

}
