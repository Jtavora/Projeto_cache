using E_Commerce.Context;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace E_Commerce.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly ECommerceContext _context;
        private readonly ICacheService _cacheService;

        public ProdutoService(ECommerceContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<Produto>> GetAllProdutosAsync()
        {
            var produtos = await _context.Produtos.ToListAsync();

            return produtos;
        }

        public async Task<Produto> GetProdutoByIdAsync(int id)
        {
            string cacheKey = $"produto:{id}";
            var cachedProduto = await _cacheService.GetCacheValueAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProduto))
            {
                return JsonSerializer.Deserialize<Produto>(cachedProduto);
            }

            var produto = await _context.Produtos.FindAsync(id);

            if (produto != null)
            {
                await _cacheService.SetCacheValueAsync(cacheKey, JsonSerializer.Serialize(produto), TimeSpan.FromMinutes(10));
            }

            return produto;
        }

        public async Task<Produto> CreateProdutoAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return produto;
        }

        public async Task<Produto> UpdateProdutoAsync(int id, Produto produto)
        {
            var existingProduto = await _context.Produtos.FindAsync(id);

            if (existingProduto != null)
            {
                existingProduto.Nome = produto.Nome;
                existingProduto.Descricao = produto.Descricao;
                existingProduto.Preco = produto.Preco;
                existingProduto.QuantidadeEstoque = produto.QuantidadeEstoque;
                existingProduto.Ativo = produto.Ativo;

                await _context.SaveChangesAsync();
            }

            return existingProduto;
        }

        public async Task<Produto> DeleteProdutoAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }

            return produto;
        }
    }
}