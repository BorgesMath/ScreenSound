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
//Adicionando O serviço de Autenticação Do Identnty

builder.Services
    .AddAuthorization(); // Adiciona o serviço de autorização


builder.Services.AddTransient<DAL<Artista>>();
builder.Services.AddTransient<DAL<Musica>>();
builder.Services.AddTransient<DAL<Genero>>();


//O ASP.NET cria esses objetos para usar no app. so
// precisa explicitar no codigo, usando o [FromServices]


builder.Services.AddEndpointsApiExplorer();

//Quando você chama esse método, ele habilita a geração de metadados para os
//endpoints do seu aplicativo, que podem
//então ser utilizados por ferramentas
//como o Swagger para criar a documentação da API.

builder.Services.AddSwaggerGen();
//O Swagger é uma ferramenta amplamente utilizada para
//gerar documentação interativa para APIs RESTful.
//Ele permite que os desenvolvedores vejam os endpoints disponíveis,
//as rotas, os métodos HTTP suportados, os parâmetros de entrada,
//e até mesmo testar as chamadas API diretamente da interface de documentação.


builder.Services.AddCors(
    options => options.AddPolicy(
        "wasm",
        policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "https://localhost:7210",
            builder.Configuration["FrontendUrl"] ?? "https://localhost:7002"])
            .AllowAnyMethod()
            .SetIsOriginAllowed(pol => true)
            .AllowAnyHeader()
            .AllowCredentials()));


// Isso serve para que e a conexão da API seja apenas usada pelo frontEnd, o ScreenSound Web

//Origens Permitidas: Apenas https://localhost:7089 e https://localhost:7015 podem fazer solicitações à API.
//Métodos Permitidos: Todos os métodos HTTP (GET, POST, etc.) são permitidos.
//Cabeçalhos Permitidos: Todos os cabeçalhos são permitidos.
//Credenciais Permitidas: Cookies e credenciais de autenticação podem ser enviados nas solicitações.


var app = builder.Build();


app.UseRouting(); // Configura o roteamento


app.UseCors("wasm");
//A linha app.UseCors("wasm") no código do ASP.NET Core é usada para aplicar uma
//política de CORS (Cross-Origin Resource Sharing) chamada "wasm" à sua aplicação.
//CORS é uma tecnologia de segurança que permite que um servidor controle quais origens
//(outras URLs) podem acessar seus recursos, como APIs, de forma segura.


app.UseStaticFiles();






app.UseAuthentication();
//Obrigatorio antes dos EndPoits,
// Para permitir ou nao mexer neles

app.UseAuthorization(); // Adiciona o middleware de autorização
#endregion

#region ChamadaDeEndPoints

app.AddEndPointsArtistas();
app.AddEndpointMusicas();
app.AddEndPointGeneros();


app.MapGroup("auth").MapIdentityApi<PessoaComAcesso>().WithTags("Autorização");

#endregion


app.UseSwagger();
app.UseSwaggerUI();
//Chamando o Swagger

//app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());

app.Run();
