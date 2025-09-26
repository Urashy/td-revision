using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using td_revision.Models;
using td_revision.Models.EntityFramework;
using td_revision.Models.Repository;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configuration CORS plus permissive pour le développement
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
            policy.WithOrigins("https://localhost:7033")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Spécification de la chaine de connexion pour le dbcontext depuis le fichier appsettings.json
builder.Services.AddDbContext<ProduitsbdContext>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Utilise les managers spécialisés (ils héritent de ManagerGenerique)
builder.Services.AddScoped<IDataRepository<Produit>, ProduitManager>();
builder.Services.AddScoped<IDataRepository<Marque>, MarqueManager>();
builder.Services.AddScoped<IDataRepository<TypeProduit>, TypeProduitManager>();
builder.Services.AddScoped<IDataRepository<Image>, ImageManager>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANT : L'ordre est crucial !
app.UseCors("AllowBlazorClient"); // CORS avant UseAuthorization
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();