using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using ScreenSound.Web;
using ScreenSound.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddTransient<ArtistaAPI>();
builder.Services.AddTransient<GeneroAPI>();
builder.Services.AddTransient<MusicaAPI>();
builder.Services.AddScoped<AuthAPI>();

// AddScoped ou AddTransient??
// Qual faz mais sentido?



builder.Services.AddHttpClient("API", client =>
{
    // Aqui fala que no arquivo de Configurações vai ter uma definição para a Url
    // Quer vai ser passada pro client
    // vai ta em wwwroot appservices.json APIServer
    client.BaseAddress = new Uri(builder.Configuration["APIServer:Url"]!);

    //Aceitar os Request só
    client.DefaultRequestHeaders.Add("Accept", "aplication/json");


});


await builder.Build().RunAsync();
