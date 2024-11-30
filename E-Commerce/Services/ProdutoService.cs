using Microsoft.EntityFrameworkCore;
using E_Commerce.Context;
using E_Commerce.Models;
using E_Commerce.Interfaces;

namespace E_Commerce.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly ECommerceContext _context;

        public ProdutoService(ECommerceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Produto>> GetAllProdutosAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<Produto> GetProdutoByIdAsync(int id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public async Task<Produto> CreateProdutoAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }
    }
}