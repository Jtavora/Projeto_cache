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
        private readonly ILoggingService _logger;

        public ProdutoService(ECommerceContext context, ICacheService cacheService, ILoggingService logger)
        {
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
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
            
            await _logger.LogAsync($"[NOVO PRODUTO] ID Produto: {produto.Id} | Nome: {produto.Nome} | Descrição: {produto.Descricao} | Preço: {produto.Preco} | Quantidade em Estoque: {produto.QuantidadeEstoque} | Ativo: {produto.Ativo}");

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

            await _logger.LogAsync($"[PRODUTO ATUALIZADO] ID Produto: {existingProduto.Id} | Nome: {existingProduto.Nome} | Descrição: {existingProduto.Descricao} | Preço: {existingProduto.Preco} | Quantidade em Estoque: {existingProduto.QuantidadeEstoque} | Ativo: {existingProduto.Ativo}");

            return existingProduto;
        }

        public async Task<Produto> DeleteProdutoAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            await _logger.LogAsync($"[PRODUTO DELETADO] ID Produto: {produto.Id} | Nome: {produto.Nome} | Descrição: {produto.Descricao} | Preço: {produto.Preco} | Quantidade em Estoque: {produto.QuantidadeEstoque} | Ativo: {produto.Ativo}");

            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }

            return produto;
        }
    }
}