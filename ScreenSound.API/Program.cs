// O ASP.NET core vem com dependencias para tratar uma API 
// Dependencias: Modelos e Dados

#region USINGS
using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Endpoints;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Dados.Modelos;
using ScreenSound.Shared.Modelos.Modelos;
using System.Net;
using System.Text.Json.Serialization;




#endregion

#region BUILDER
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
    options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Como o Artistas e Musica tem dependencias entre si o RefenceHandler vai resolver isso
// para que o Json consiga criar uma lista com os dados quando for necessario 




builder.Services.AddDbContext<ScreenSoundContext>();


builder.Services
        .AddIdentityApiEndpoints<PessoaComAcesso>()
        .AddEntityFrameworkStores<ScreenSoundContext>();
//Aidiconando o Identity EndPoint, gerado automanticamente



builder.Services
    .AddAuthentication();
//Adicionando O servi�o de Autentica��o Do Identnty

builder.Services
    .AddAuthorization(); // Adiciona o servi�o de autoriza��o


builder.Services.AddTransient<DAL<Artista>>();
builder.Services.AddTransient<DAL<Musica>>();
builder.Services.AddTransient<DAL<Genero>>();


//O ASP.NET cria esses objetos para usar no app. so
// precisa explicitar no codigo, usando o [FromServices]


builder.Services.AddEndpointsApiExplorer();

//Quando voc� chama esse m�todo, ele habilita a gera��o de metadados para os
//endpoints do seu aplicativo, que podem
//ent�o ser utilizados por ferramentas
//como o Swagger para criar a documenta��o da API.

builder.Services.AddSwaggerGen();
//O Swagger � uma ferramenta amplamente utilizada para
//gerar documenta��o interativa para APIs RESTful.
//Ele permite que os desenvolvedores vejam os endpoints dispon�veis,
//as rotas, os m�todos HTTP suportados, os par�metros de entrada,
//e at� mesmo testar as chamadas API diretamente da interface de documenta��o.


builder.Services.AddCors(
    options => options.AddPolicy(
        "wasm",
        policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "https://localhost:7210",
            builder.Configuration["FrontendUrl"] ?? "https://localhost:7002"])
            .AllowAnyMethod()
            .SetIsOriginAllowed(pol => true)
            .AllowAnyHeader()
            .AllowCredentials()));


// Isso serve para que e a conex�o da API seja apenas usada pelo frontEnd, o ScreenSound Web

//Origens Permitidas: Apenas https://localhost:7089 e https://localhost:7015 podem fazer solicita��es � API.
//M�todos Permitidos: Todos os m�todos HTTP (GET, POST, etc.) s�o permitidos.
//Cabe�alhos Permitidos: Todos os cabe�alhos s�o permitidos.
//Credenciais Permitidas: Cookies e credenciais de autentica��o podem ser enviados nas solicita��es.


var app = builder.Build();


app.UseRouting(); // Configura o roteamento


app.UseCors("wasm");
//A linha app.UseCors("wasm") no c�digo do ASP.NET Core � usada para aplicar uma
//pol�tica de CORS (Cross-Origin Resource Sharing) chamada "wasm" � sua aplica��o.
//CORS � uma tecnologia de seguran�a que permite que um servidor controle quais origens
//(outras URLs) podem acessar seus recursos, como APIs, de forma segura.


app.UseStaticFiles();






app.UseAuthentication();
//Obrigatorio antes dos EndPoits,
// Para permitir ou nao mexer neles

app.UseAuthorization(); // Adiciona o middleware de autoriza��o
#endregion

#region ChamadaDeEndPoints

app.AddEndPointsArtistas();
app.AddEndpointMusicas();
app.AddEndPointGeneros();


app.MapGroup("auth").MapIdentityApi<PessoaComAcesso>().WithTags("Autoriza��o");

#endregion


app.UseSwagger();
app.UseSwaggerUI();
//Chamando o Swagger

//app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());

app.Run();
