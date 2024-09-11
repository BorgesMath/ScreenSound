using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using ScreenSound.Web;
using ScreenSound.Web.Services;
using System.Net;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthAPI>();
builder.Services.AddScoped<AuthAPI>(sp => (AuthAPI)
    sp.GetRequiredService<AuthenticationStateProvider>());



// Cookie Handler � uma fun��o para que sej possivel lidar com os Cookies, passar
// no AddHttpClient
builder.Services.AddScoped<CokieHandler>();
builder.Services.AddTransient<ArtistaAPI>();
builder.Services.AddTransient<GeneroAPI>();
builder.Services.AddTransient<MusicaAPI>();


builder.Services.AddHttpClient("API", client =>
{
    // Aqui fala que no arquivo de Configura��es vai ter uma defini��o para a Url
    // Quer vai ser passada pro client
    // vai ta em wwwroot appservices.json APIServer
    client.BaseAddress = new Uri(builder.Configuration["APIServer:Url"]!);

    //Aceitar os Request s�
    client.DefaultRequestHeaders.Add("Accept", "aplication/json");

    
}).AddHttpMessageHandler<CokieHandler>();



await builder.Build().RunAsync();
