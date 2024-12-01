using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Models;
using E_Commerce.Interfaces;

namespace E_Commerce.Context
{
    public class ECommerceContext : DbContext
    {
        private readonly IMessagePublisher _messagePublisher;

        public ECommerceContext(DbContextOptions<ECommerceContext> options, IMessagePublisher messagePublisher)
            : base(options)
        {
            _messagePublisher = messagePublisher;
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(); // Habilita proxies para carregamento preguiçoso
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Venda>()
                .Property(v => v.Status)
                .HasConversion<string>();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            // Captura as alterações nas entidades de Produto e Venda
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Produto || e.Entity is Venda)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            foreach (var entityEntry in entries)
            {
                if (entityEntry.Entity is Produto produto)
                {
                    string changeType = entityEntry.State.ToString();
                    string message = $"{{ \"Produto\": \"{produto.Id}\", \"ChangeType\": \"{changeType}\" }}";
                    await _messagePublisher.PublishAsync(message);

                    if (entityEntry.State == EntityState.Added)
                    {
                        produto.DataCriacao = DateTime.Now;
                    }
                    produto.DataAtualizacao = DateTime.Now;
                }

                if (entityEntry.Entity is Venda venda)
                {
                    if (entityEntry.State == EntityState.Added)
                    {
                        venda.DataCriacao = DateTime.Now;
                    }
                    venda.DataAtualizacao = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}