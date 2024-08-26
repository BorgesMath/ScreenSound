namespace ScreenSound.Web.Requests;

public record ArtistaRequest(string Nome, string Bio);
//Em C#, record é uma palavra-chave usada para definir um tipo especial
//de classe que é imutável por padrão e é especialmente útil para representar
//objetos que são basicamente coleções de dados, como um DTO (Data Transfer Object)