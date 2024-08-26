using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints;


public static class ArtistasExtensions
{

    public static void AddEndPointsArtistas(this 
        WebApplication app)
    {
        #region Artistas
        // quando fizer um GET no caminho /Artistas vai retornar uma lista de artistas
        app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
        // FromServices usado para mostra que sera usado o 
        // o Objeto DAL criado no Transient
        {

            //DAL<Artista> dal = new(new ScreenSoundContext()); => Resolvido pelo Transient e DbContext!!
            // Chamando o DAL para Artistas que manipula o BD, para ele é necessario 
            // passar a conexão 
            var listaDeArtistas = dal.Listar();

            if (listaDeArtistas is null)
            {
                return Results.NotFound();
            }
            var listaDeArtistaResponse = EntityListToResponseList(listaDeArtistas);
            // O Response serve para retornar apenas o necessario do Artista
            // A função EntityListToResponseList cria um lista de ArtistaResponse
            // a partir da Lista de Artista




            return Results.Ok(listaDeArtistaResponse);
            //Retorna um StatusCode 200, significando que ta Okay e 
            // retorna uma lista de Artistas

        }

        );

        // quando fizer um GET no caminho /Artistas/{nome} vai retornar o Artista com o nome passado no Http
        app.MapGet("/Artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
        {
            var artista = dal.RecuperarPor(a => a.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
            // O recuperarPor passa uma função por cada Artista (Definido na criação do DAL) dentro do DB
            // essa função é o Equals, então ele retorna o "a" que é o artistas do Equals.
            // O StringCOmaprison compara os nomes ignorando UpperCases e etc

            if (artista == null)
            {
                return Results.NotFound();
                // Results.NotFound retornar um StatusCode 404
                // que acontece caso seja nulo
            }

            return Results.Ok(EntityToResponse(artista));
            // Results.Ok retorna um 202 e o Response Artista
            // O Artista Response é um record da maneira que deve e o que
            // deve ser retornado como resposta
            // A função enteitytoresponse trasforma artista em artistaRespose
        }
        );

        // quando fizer um POST no caminho /Artistas/ vai criar o Artista com o os dados passados no Http
        app.MapPost("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
        // O FromBody explicita que o artista                                 //ArtistaRequest para que
        // vai vir da requisição Http                                         // Seja pedido no Http só o 
        {                                                                     // Necessario 

            Artista artista = new(artistaRequest.Nome, artistaRequest.Bio);
            dal.Adicionar(artista);
            return Results.Ok();
        }
        );

        // quando fizer um DELETE vai apagar o artista dependendo do id dele
        app.MapDelete("/Artistas/{id}", ([FromServices] DAL<Artista> dal, int id) => {
            var artista = dal.RecuperarPor(a => a.Id == id);
            //Procura o artista por id e salva na var artista
            if (artista is null)
            {
                return Results.NotFound();
            }
            dal.Deletar(artista);
            return Results.NoContent();
            //Retornar um StatusCode 204 falando que foi apagado tudo
            //a respeito do necessario. 

        });



        // Atualiza um Artista, o PUT passa um Artista "novo" que sera posto nos dados de outro onde 
        // foi passado dados. 
        app.MapPut("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artistaEdit) => {
                                                                                // Assim vai herdar do Record 
            var artistaAAtualizar = dal.RecuperarPor(a => a.Id == artistaEdit.Id);
            if (artistaAAtualizar is null)
            {
                return Results.NotFound();
            }
            artistaAAtualizar.Nome = artistaEdit.Nome;
            artistaAAtualizar.Bio = artistaEdit.Bio;

            dal.Atualizar(artistaAAtualizar);
            return Results.Ok();
        });


        #endregion

    }

    #region FuncoesDeAjuda
    private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
    {
        return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
    }

    private static ArtistaResponse EntityToResponse(Artista artista)
    {
        return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
    }

    #endregion


}
