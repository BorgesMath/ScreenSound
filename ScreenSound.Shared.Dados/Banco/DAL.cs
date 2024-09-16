using Microsoft.EntityFrameworkCore;
using ScreenSound.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenSound.Shared.Dados.Banco
{
    public class DAL<T> where T : class
    {
        private readonly ScreenSoundContext _context;

        // Construtor que recebe o contexto e atribui à variável privada _context
        public DAL(ScreenSoundContext context)
        {
            _context = context;
        }

        public IEnumerable<T> Listar()
        {
            return _context.Set<T>().AsEnumerable();
        }

        public void Adicionar(T objeto)
        {
            _context.Set<T>().Add(objeto);
            _context.SaveChanges();
        }

        public void Atualizar(T objeto)
        {
            _context.Set<T>().Update(objeto);
            _context.SaveChanges();
        }

        public void Deletar(T objeto)
        {
            _context.Set<T>().Remove(objeto);
            _context.SaveChanges();
        }

        public T? RecuperarPor(Func<T, bool> condicao)
        {
            return _context.Set<T>().FirstOrDefault(condicao);
        }

        public IEnumerable<T> ListarPor(Func<T, bool> condicao)
        {
            return _context.Set<T>().Where(condicao).AsEnumerable();
        }

        // Método que inclui Artista e Generos ao listar músicas
        public IEnumerable<Musica> ListarComArtistasEGeneros()
        {
            return _context.Musicas.Include(m => m.Artista).Include(m => m.Generos).AsEnumerable();
        }
    }
}
