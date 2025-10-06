using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nasa.API.Controllers.Base.Filters;
using Nasa.IoC;
using Nasa.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      // Permite qualquer origem
            .AllowAnyMethod()      // Permite GET, POST, PUT, DELETE, etc
            .AllowAnyHeader();     // Permite qualquer header
    });
});



// Configuração do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nasa API", Version = "v1" });
});

// Configuração dos Controllers e JSON
builder.Services.AddScoped<IpWhitelistFilter>();
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<NasaExceptionFilter>();
        options.Filters.Add<IpWhitelistFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Registro dos serviços da aplicação
BootStrapper.Inject(builder.Services, builder.Configuration);

var app = builder.Build();

// Configuração do pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nasa API V1");
});

// Remover UseHttpsRedirection em produção (Railway gerencia SSL)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Fallback apenas se existir index.html
if (File.Exists(Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "index.html")))
{
    app.MapFallbackToFile("index.html");
}

app.Run();