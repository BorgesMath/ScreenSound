namespace ScreenSound.API.Requests;

using System.ComponentModel.DataAnnotations;
public record MusicaRequest([Required] string Nome, [Required] int ArtistaId, int AnoLancamento, ICollection<GeneroRequest>? Generos = null);
                            // Quando passado para o Http                                                       // Valor padrão de Null 
                            // Vai avisar que esse dado é obrigatorio 