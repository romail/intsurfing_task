using BCMS_UI.Components;
using BCMS_UI.Services;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5202/api") });
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<HubConnectionBuilder>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddServerSideBlazor(options =>
 {
     options.DetailedErrors = true;
 });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();