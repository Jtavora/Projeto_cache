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
            string cacheKey = "produtos";
            var cachedProdutos = await _cacheService.GetCacheValueAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProdutos))
            {
                return JsonSerializer.Deserialize<IEnumerable<Produto>>(cachedProdutos);
            }

            var produtos = await _context.Produtos.ToListAsync();

            if (produtos != null)
            {
                await _cacheService.SetCacheValueAsync(cacheKey, JsonSerializer.Serialize(produtos), TimeSpan.FromMinutes(10));
            }

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

            // Invalidate cache
            await _cacheService.SetCacheValueAsync("produtos", string.Empty);

            return produto;
        }
    }
}