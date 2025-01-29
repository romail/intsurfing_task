using System.Threading.RateLimiting;
using BCMS_API;
using BCMS_API.Controllers;
using BCMS_API.Services;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<BookContext>(options =>
    options.UseInMemoryDatabase("BookCatalog"));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = true;
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BCMS API", Version = "v1" });
});
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookContext>();
    dbContext.SeedDatabase();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseCors();
app.UseRouting();

app.MapHub<BookHub>("/hub/bookhub", options =>
{
    options.Transports = HttpTransportType.WebSockets;
    options.AllowStatefulReconnects = true;
});

app.MapBookEndpoints();

app.Run();

public class BookHub : Hub
{
}

