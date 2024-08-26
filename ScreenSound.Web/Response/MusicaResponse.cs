using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.Web.Response;

public record MusicaResponse(int Id, string Nome, int ArtistaId, string NomeArtista);
