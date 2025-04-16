using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FrutasApp.Models;

namespace FrutasApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Fruta> Frutas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configura a conexão com o banco de dados PostgreSQL
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=frutasdb;Username=postgres;Password=1234");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            // Configurar o enum para ser armazenado como string
            modelBuilder.Entity<Fruta>()
                .Property(f => f.Sabor)
                .HasConversion<string>();

            // Configurar o relacionamento entre Categoria e Fruta
            modelBuilder.Entity<Fruta>()
                .HasOne(f => f.Categoria)      // Uma fruta tem uma categoria
                .WithMany(c => c.Frutas)       // Uma categoria tem muitas frutas
                .HasForeignKey(f => f.CategoriaId); // Usando CategoriaId como chave estrangeira
        }
    }
}
