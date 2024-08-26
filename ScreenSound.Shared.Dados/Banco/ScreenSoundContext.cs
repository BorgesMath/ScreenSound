using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreenSound.Banco;
public class ScreenSoundContext: DbContext
{

    //  DbSet vai fazer com que essas classes se tornem tabelas
    // no banco de dados, O nome da tabelas vai ser o nome da propridade
    public DbSet<Artista> Artistas { get; set; }
    public DbSet<Musica> Musicas { get; set; }
    public DbSet<Genero> Generos { get; set; }

    private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ScreenSoundV0;Integrated Security=True;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(connectionString)
            .UseLazyLoadingProxies(false);
        // tive que mudar para false, estava dando errado quando usava o get no /artistas/nome

        //erro: O erro que você está enfrentando está relacionado à desserialização de objetos dinâmicos
        //criados pelo Castle Proxy. Isso pode ocorrer quando você está usando proxies gerados por frameworks
        //como o Entity Framework ou o Castle DynamicProxy, e o System.Text.Json não consegue lidar com
        //a desserialização desses proxies.


        //Quando você define.UseLazyLoadingProxies(false), você desativa o Lazy Loading.
        //Isso significa que todas as entidades relacionadas serão carregadas imediatamente
        //junto com a entidade principal, em vez de serem carregadas sob demanda.


    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Musica>()
            .HasMany(c => c.Generos).WithMany(c => c.Musicas);
        //Aqui é feito para relacionar musicas e generos
        // A entity Musica tem muitos generos relacionados com muitas musicas.

        // Isso vai criar uma tabela de conexão entre elas

        modelBuilder.Entity<Musica>()
    .HasOne(m => m.Artista)
    .WithMany(a => a.Musicas)
    .HasForeignKey(m => m.ArtistaId);
    //Aqui para fazer a conexao entre artista e Musica

    }




}
