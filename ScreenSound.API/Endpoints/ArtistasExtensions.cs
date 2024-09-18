using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Shared.Dados.Banco;
using ScreenSound.Shared.Dados.Modelos;
using ScreenSound.Shared.Modelos.Modelos;
using System.Security.Claims;

namespace ScreenSound.API.Endpoints;


public static class ArtistasExtensions
{

    public static void AddEndPointsArtistas(this 
        WebApplication app)
    {

        var groupBuilder = app.MapGroup("artistas")
            .RequireAuthorization()
            .WithTags("Artistas");

        #region Artistas
        // quando fizer um GET no caminho /Artistas vai retornar uma lista de artistas
        groupBuilder.MapGet("", ([FromServices] DAL<Artista> dal) =>
        // FromServices usado para mostra que sera usado o 
        // o Objeto DAL criado no Transient
        {

            //DAL<Artista> dal = new(new ScreenSoundContext()); => Resolvido pelo Transient e DbContext!!
            // Chamando o DAL para Artistas que manipula o BD, para ele é necessario 
            // passar a conexão 
            var listaDeArtistas = dal.ListarComAvaliacao();

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

        });

        // quando fizer um GET no caminho /Artistas/{nome} vai retornar o Artista com o nome passado no Http
        groupBuilder.MapGet("{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
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
        groupBuilder.MapPost("", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
        // O FromBody explicita que o artista                                 //ArtistaRequest para que
        // vai vir da requisição Http                                         // Seja pedido no Http só o 
        {                                                                     // Necessario 

            Artista artista = new(artistaRequest.Nome, artistaRequest.Bio);
            dal.Adicionar(artista);
            return Results.Ok();
        }
        );

        // quando fizer um DELETE vai apagar o artista dependendo do id dele
        groupBuilder.MapDelete("{id}", ([FromServices] DAL<Artista> dal, int id) => {
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
        groupBuilder.MapPut("", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artistaEdit) => {
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



        #region ArtistasAvaliacao

        groupBuilder.MapPost("/avaliacao", (
            HttpContext context,
            [FromBody] AvaliacaoArtistaRequest request,
            [FromServices] DAL<Artista> dalArtista,
            [FromServices] DAL<PessoaComAcesso> dalPessoa
            ) =>
        {
            var artista = dalArtista.RecuperarPor(a => a.Id == request.ArtistaID);
            if (artista is null) return Results.NotFound();

            var email = context.User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                            ?? throw new InvalidOperationException("Pessoa nao conectada");


            var pessoa = dalPessoa.RecuperarPor(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Pessoa nao conectada");


            var avaliacao = artista.Avaliacao.FirstOrDefault(a => a.ArtistaID == artista.Id && a.PessoaID == pessoa.Id);

            if (avaliacao is null) artista.AdicionarNota(pessoa.Id, request.Nota);

            else avaliacao.Nota = request.Nota;

            dalArtista.Atualizar(artista);

            return Results.Created();

        });





        groupBuilder.MapGet("{id}/avaliacao", (
    int id,
    HttpContext context,
    [FromServices] DAL<Artista> dalArtista,
    [FromServices] DAL<PessoaComAcesso> dalPessoa
    ) =>
        {
            var artista = dalArtista.RecuperarComAvaliacao(a => a.Id == id);
            if (artista is null) return Results.NotFound();

            var email = context.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                ?? throw new InvalidOperationException("Não foi encontrado o email da pessoa logada");

            var pessoa = dalPessoa.RecuperarPor(p => p.Email!.Equals(email))
                ?? throw new InvalidOperationException("Não foi encontrado o email da pessoa logada");

            var avaliacao = artista
                .Avaliacao
                .FirstOrDefault(a => a.ArtistaID == id && a.PessoaID == pessoa.Id);

            if (avaliacao is null) return Results.Ok(new AvaliacaoArtistaResponse(0, id));
            else return Results.Ok(new AvaliacaoArtistaResponse(avaliacao.Nota, id));
        });

        #endregion
    }

    #region FuncoesDeAjuda
    private static List<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
    {
        return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
    }

    private static ArtistaResponse EntityToResponse(Artista artista)
    {
        return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil)
        {
            Cassificacao = artista.Avaliacao
                            .Select(a => a.Nota)
                            .DefaultIfEmpty(0)
                            .Average()
        };
    }


    #endregion


}
