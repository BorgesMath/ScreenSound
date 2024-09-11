// O ASP.NET core vem com dependencias para tratar uma API 
// Dependencias: Modelos e Dados

#region USINGS
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Endpoints;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Dados.Banco;
using ScreenSound.Shared.Dados.Modelos;
using ScreenSound.Shared.Modelos.Modelos;
using System.Text.Json.Serialization;


#endregion

#region BUILDER
var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddCors();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
    options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Como o Artistas e Musica tem dependencias entre si o RefenceHandler vai resolver isso
// para que o Json consiga criar uma lista com os dados quando for necessario 

builder.Services.AddDbContext<ScreenSoundContext>();


builder.Services
    .AddIdentityApiEndpoints<PessoaComAcesso>()
    .AddEntityFrameworkStores<ScreenSoundContext>();



builder.Services.AddAuthorization();

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


//builder.Services.AddCors(
//    options => options.AddPolicy(
//        "wasm",
//        policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "https://localhost:7089",
//            builder.Configuration["FrontendUrl"] ?? "https://localhost:7015"])
//            .AllowAnyMethod()
//            .SetIsOriginAllowed(pol => true)
//            .AllowAnyHeader()
//            .AllowCredentials()));



builder.Services.AddCors(options => options.AddPolicy(
    "wasm",
    policy => policy
        .WithOrigins(
        [
            builder.Configuration["BackendUrl"] ?? "https://localhost:7089",
            builder.Configuration["FrontendUrl"] ?? "https://localhost:7015",
            "https://localhost:7210", // Outra porta para o backend
            "https://localhost:7002"  // Outra porta para o frontend
        ])
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()));


var app = builder.Build();

#endregion

#region UseConfigs

app.UseCors("wasm");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();



#endregion


#region ChamadaDeEndPoints

app.AddEndPointsArtistas();
app.AddEndpointMusicas();
app.AddEndPointGeneros();


app.MapGroup("auth").MapIdentityApi<PessoaComAcesso>().WithTags("Autorizacao");

app.MapPost("auth/logout", async ([FromServices] SignInManager<PessoaComAcesso> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();

}).RequireAuthorization().WithTags("Autorizacao");

#endregion


app.UseSwagger();
app.UseSwaggerUI();
//Chamando o Swagger


app.Run();


