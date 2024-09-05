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





//builder.Services.AddIdentity<PessoaComAcesso, PerfilDeAcesso>(options =>
//{
//    // Configurando as op��es
//    options.SignIn.RequireConfirmedAccount = false; // Desativa a necessidade de conta confirmada
//    options.Tokens.PasswordResetTokenProvider = null; // Desativa recupera��o de senha
//    options.Tokens.EmailConfirmationTokenProvider = null; // Desativa confirma��o por e-mail
//    options.SignIn.RequireConfirmedEmail = false;
//    options.Password.RequireDigit = false;             // N�o exige d�gitos na senha
//    options.Password.RequiredLength = 1;               // Tamanho m�nimo de 1 caractere
//    options.Password.RequireLowercase = false;         // N�o exige letras min�sculas
//    options.Password.RequireUppercase = false;         // N�o exige letras mai�sculas
//    options.Password.RequireNonAlphanumeric = false;   // N�o exige caracteres especiais
//})
//.AddEntityFrameworkStores<ScreenSoundContext>();



builder.Services.AddAuthorization();

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
        policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "https://localhost:7089",
            builder.Configuration["FrontendUrl"] ?? "https://localhost:7015"])
            .AllowAnyMethod()
            .SetIsOriginAllowed(pol => true)
            .AllowAnyHeader()
            .AllowCredentials()));

var app = builder.Build();

#endregion

#region UseConfigs

app.UseCors("wasm");
app.UseStaticFiles();
app.UseAuthorization();

#endregion


#region ChamadaDeEndPoints

app.AddEndPointsArtistas();
app.AddEndpointMusicas();
app.AddEndPointGeneros();


app.MapGroup("auth").MapIdentityApi<PessoaComAcesso>().WithTags("Autorizacao");             

#endregion


app.UseSwagger();
app.UseSwaggerUI();
//Chamando o Swagger


app.Run();
