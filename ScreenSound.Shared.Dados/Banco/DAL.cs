
using Microsoft.EntityFrameworkCore;
using ScreenSound.Modelos;
using ScreenSound.Shared.Dados.Banco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSound.Banco;
public class DAL<T>(ScreenSoundContext context) where T : class
    // Clase publica para ser acessada pelos outros projetos
{
    private readonly ScreenSoundContext context = context;

    public IEnumerable<T> Listar()
    {
        return [.. context.Set<T>()];
    }
    public void Adicionar(T objeto)
    {
        context.Set<T>().Add(objeto);
        context.SaveChanges();
    }
    public void Atualizar(T objeto)
    {
        context.Set<T>().Update(objeto);
        context.SaveChanges();
    }
    public void Deletar(T objeto)
    {
        context.Set<T>().Remove(objeto);
        context.SaveChanges();
    }

    public T? RecuperarPor(Func<T, bool> condicao)
    {
        return context.Set<T>().FirstOrDefault(condicao);
    }

    public IEnumerable<T> ListarPor(Func<T, bool> condicao)
    {
        return context.Set<T>().Where(condicao);
    }

    public IEnumerable<Musica> ListarComArtistasEGeneros()
    {
        return context.Musicas.Include(m => m.Artista).Include(m => m.Generos).AsEnumerable();
    }
    //CUIDADOOO
    // ESTOU TENDO UM ERRO POR CONTA DO LAZY PROXY
    // NAO ESTAVA CARREGANDO ARTISTAS E GENEROS!

}
