using Jwt_autenticacao.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using jwt_autenticacao.Application.Cache;
using Microsoft.AspNetCore.Hosting;
using jwt_autenticacao.Application.Eventos;

var builder = WebApplication.CreateBuilder(args);

using IHost host = Host.CreateDefaultBuilder(args).Build();
IConfiguration config = host.Services.GetRequiredService<IConfiguration>();


// Add services to the container.

builder.Services.AddControllers();


//Configuração com o Jwt
builder.Services.AddAuthentication
    (JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "var",
            ValidAudience = "var",
            IssuerSigningKey = new SymmetricSecurityKey
          (Encoding.UTF8.GetBytes("5ea4cfa4-badf-4763-ac0e-3bd48fca4c76"))
        };
    });



// Conexão com o sql server
builder.Services.AddDbContext<PessoaContext>
    (options => options.UseSqlServer("Server=127.0.0.1;Database=Pessoa;User id=sa;Password=Teste01&"));

builder.Services.AddStackExchangeRedisCache(options => options.Configuration = config.GetValue<string>("Redis:ConnectionString"));




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Adiconando interface
builder.Services.AddSingleton<ICache, Cache>();
builder.Services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();

var app = builder.Build();

// Configure the HTTP request pwwipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

