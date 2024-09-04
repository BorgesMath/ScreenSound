using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;

namespace ScreenSound.API.Endpoints;

public static class MusicasExtensions
{
    public static void AddEndpointMusicas(this WebApplication app)
    {
        #region Musicas
        app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
        // quando fizer um GET no caminho /Musicas vai retornar uma lista de artistas
        {
            // Chamando o DAL para Musicas que manipula o BD, para ele é necessario 
            // passar a conexão 
            var lista = dal.ListarComArtistasEGeneros();
            // ESTOU TENDO UM ERRO POR CONTA DO LAZY PROXY
            // NAO ESTAVA CARREGANDO ARTISTAS E GENEROS!

            //var lista = dal.Listar();
            if (lista == null)
            {
                return Results.NotFound();
            }


            var musicaListResponse = EntityListToResponseList(lista);


            //return Results.Ok(lista);
            return Results.Ok(musicaListResponse);
            // retorna uma lista de Musicas

        }

        );


        app.MapGet("/Musicas/{nome}", ([FromServices] DAL<Musica> dal, string nome) =>

        {
            var lista = dal.ListarComArtistasEGeneros();
            // Problema, estou tendo que puxar a lista inteira de musicas o que diminui a qualidade.
            //Esse problema vem do lazy proxy

            var musica = lista.FirstOrDefault(a => a.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

            if (musica == null)
            {
                return Results.NotFound();
            }

            //return Results.Ok(musica);s
            return Results.Ok(EntityToResponse(musica));

        }

        );


        app.MapPost("/Musicas", ([FromServices] DAL<Musica> dal, [FromServices] DAL<Genero> dalGenero, [FromBody] MusicaRequest musicaRequest) =>
        {
            Musica musica = new(musicaRequest.Nome)
            {
                AnoLancamento = musicaRequest.AnoLancamento,
                ArtistaId = musicaRequest.ArtistaId,
                Generos = musicaRequest.Generos is not null ?
                GeneroRequestConverter(musicaRequest.Generos, dalGenero) : []
            };
            dal.Adicionar(musica);
            return Results.Ok();
        });


        app.MapDelete("/Musicas/{id}", ([FromServices] DAL<Musica> dal, int id) =>
        {
            var musica = dal.RecuperarPor(a => a.Id == id);
            if (musica is null)
            {
                return Results.NotFound();
            }
            dal.Deletar(musica);
            return Results.NoContent();


        });


        app.MapPut("/Musicas", ([FromServices] DAL<Musica> dal, [FromBody] MusicaRequestEdit musicaEdit) =>
        {
            var musicaAAtualizar = dal.RecuperarPor(a => a.Id == musicaEdit.Id);
            if (musicaAAtualizar is null)
            {
                return Results.NotFound();
            }
            musicaAAtualizar.Nome = musicaEdit.Nome;
            musicaAAtualizar.AnoLancamento = musicaEdit.AnoLancamento;


            dal.Atualizar(musicaAAtualizar);
            return Results.Ok();
        });
        #endregion
    }


    #region FuncoesApoio



    private static List<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos, DAL<Genero> dalGenero)
    {

        var listaDeGeneros = new List<Genero>();  

        foreach (var genero in generos)
        {
            var entity = RequestToEntity(genero);
            // primeiro passo transformar de generoRequest para genero

            var Gen = dalGenero.RecuperarPor(g => g.Nome!.Equals(genero.Nome, StringComparison.OrdinalIgnoreCase));
            
            if (Gen is not null)
            {
                listaDeGeneros.Add(Gen);
            }
            else
            {
                listaDeGeneros.Add(entity);
            }
            //Passamos para o método GeneroRequestConverter() também o DAL<Genero>
            //para fazer a consulta.Quando fazemos a consulta na base de dados por um
            //gênero a partir da música, ou pelo nome, o entity marca a entidade como
            //uma entidade rastreável se ele a recuperar.

            //Então, ele é manipulado pelo entity.Caso seja necessário alterar essa
            //entidade que está marcada, ele vai atualizar a informação, ou seja,
            //vai cadastrar novamente o item, como acontecia antes.


        }
        return listaDeGeneros;




        //return generos.Select(a => RequestToEntity(a)).ToList();
    }

    private static Genero RequestToEntity(GeneroRequest a)
    {
        return new Genero()
        {
            Nome = a.Nome,
            Descricao = a.Descricao
        };
    }

    private static List<MusicaResponse> EntityListToResponseList(IEnumerable<Musica> musicaList)
    {
        return musicaList.Select(a => EntityToResponse(a)).ToList();
    }

    private static MusicaResponse EntityToResponse(Musica musica)
    {
        return new MusicaResponse(musica.Id, musica.Nome!,  musica.Artista!.Id, musica.Artista.Nome, musica.AnoLancamento);
    }

    #endregion
}


