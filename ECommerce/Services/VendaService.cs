using E_Commerce.Context;
using E_Commerce.DTOsCustom;
using E_Commerce.Models;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Interfaces;
using System.Text.Json;

namespace E_Commerce.Services
{
    public class VendaService : IVendaService
    {
        private readonly ECommerceContext _context;
        private readonly ILoggingService _logger;
        private readonly ICacheService _cacheService;

        public VendaService(ECommerceContext context, ILoggingService logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<Venda>> GetAllVendasAsync()
        {
            return await _context.Vendas
                .Include(v => v.ItensVenda)
                .ThenInclude(iv => iv.Produto)
                .ToListAsync();
        }

        public async Task<Venda> GetVendaByIdAsync(int id)
        {
            string cacheKey = $"venda:{id}";
            var cachedVenda = await _cacheService.GetCacheValueAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedVenda))
            {
                return JsonSerializer.Deserialize<Venda>(cachedVenda);
            }

            var venda = await _context.Vendas
                .Include(v => v.ItensVenda)
                .ThenInclude(iv => iv.Produto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda != null)
            {
                await _cacheService.SetCacheValueAsync(cacheKey, JsonSerializer.Serialize(venda), TimeSpan.FromMinutes(10));
            } 

            return venda;
        }

        public async Task<Venda> CreateVendaAsync(VendaDTO vendaDTO)
        {
            if (vendaDTO.Produtos == null || !vendaDTO.Produtos.Any())
            {
                throw new ArgumentException("A venda deve conter pelo menos um produto.");
            }

            var produtoIds = vendaDTO.Produtos.Select(p => p.ProdutoId).ToList();
            var produtos = await _context.Produtos
                .Where(p => produtoIds.Contains(p.Id))
                .ToListAsync();

            if (produtos.Count != vendaDTO.Produtos.Count)
            {
                throw new KeyNotFoundException("Um ou mais produtos não foram encontrados.");
            }

            var venda = new Venda
            {
                UsuarioId = vendaDTO.UsuarioId,
                Cliente = vendaDTO.Cliente,
                Pagamento = vendaDTO.Pagamento,
                DataVenda = DateTime.Now,
                Status = StatusVenda.Pendente
            };

            decimal total = 0;

            foreach (var produtoDTO in vendaDTO.Produtos)
            {
                var produto = produtos.First(p => p.Id == produtoDTO.ProdutoId);

                if (produtoDTO.Quantidade > produto.QuantidadeEstoque || !produto.Ativo)
                {
                    throw new ArgumentException($"Produto {produto.Nome} não está disponível.");
                }

                var itemVenda = new ItemVenda
                {
                    ProdutoId = produto.Id,
                    Venda = venda,
                    Quantidade = produtoDTO.Quantidade,
                    PrecoUnitario = produto.Preco
                };

                total += itemVenda.PrecoUnitario * itemVenda.Quantidade;
                venda.ItensVenda.Add(itemVenda);
            }

            venda.Total = total;

            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();

            await _logger.LogAsync($"[NOVA VENDA] ID Venda: {venda.Id} | ID Usuario: {venda.UsuarioId} | Total: {venda.Total} | Cliente: {venda.Cliente}");

            return venda;
        }

        public async Task<bool> UpdateVendaAsync(int id, Venda venda)
        {
            if (id != venda.Id)
            {
                return false;
            }

            _context.Entry(venda).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVendaAsync(int id)
        {
            var venda = await _context.Vendas.FindAsync(id);

            if (venda == null)
            {
                return false;
            }

            _context.Vendas.Remove(venda);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}