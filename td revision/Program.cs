using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using td_revision.Models;
using td_revision.Models.EntityFramework;
using td_revision.Models.Repository;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("LocalConnection");

builder.Services.AddDbContext<ProduitsbdContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy =>
        {
            policy.SetIsOriginAllowed(origin =>
                new Uri(origin).Host == "localhost")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProduitsbdContext>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ========== ENREGISTREMENT DES REPOSITORIES ==========

// Repositories avec recherche par nom (INamedRepository)
builder.Services.AddScoped<INamedRepository<Produit>, ProduitManager>();
builder.Services.AddScoped<INamedRepository<Marque>, MarqueManager>();
builder.Services.AddScoped<INamedRepository<TypeProduit>, TypeProduitManager>();

// Repository sans recherche par nom (IRepository seulement)
builder.Services.AddScoped<IRepository<Image>, ImageManager>();


// Pour la compatibilité avec SeedController qui utilise encore l'ancienne interface
// On enregistre aussi les IRepository<T> pour les entités qui ont INamedRepository
builder.Services.AddScoped<IRepository<Produit>>(sp => sp.GetRequiredService<INamedRepository<Produit>>());
builder.Services.AddScoped<IRepository<Marque>>(sp => sp.GetRequiredService<INamedRepository<Marque>>());
builder.Services.AddScoped<IRepository<TypeProduit>>(sp => sp.GetRequiredService<INamedRepository<TypeProduit>>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazorClient");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();