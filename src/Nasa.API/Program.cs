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
    options.AddPolicy("NasaPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition");
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Washflow API V1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("QKDPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configuração de fallback para SPA (se necessário)
app.MapFallbackToFile("index.html");

app.Run();