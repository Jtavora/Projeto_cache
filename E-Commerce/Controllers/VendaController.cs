using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using E_Commerce.Context;
using E_Commerce.Models;
using E_Commerce.DTOsCustom;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendaController : ControllerBase
    {
        private readonly ECommerceContext _context;

        public VendaController(ECommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Venda>> Get()
        {
            var vendas = _context.Vendas.ToList();

            if (vendas.Count == 0)
            {
                return NotFound();
            }

            return Ok(vendas);
        }

        [HttpGet("{id}")]
        public ActionResult<Venda> Get(int id)
        {
            var venda = _context.Vendas
                .Include(v => v.ItensVenda)
                .ThenInclude(iv => iv.Produto) // Opcional: Inclui também os produtos, se necessário
                .FirstOrDefault(v => v.Id == id);

            if (venda == null)
            {
                return NotFound();
            }

            return venda;
        }

        [HttpPost]
        public ActionResult<Venda> Post([FromBody] VendaDTO vendaDTO)
        {
            if (vendaDTO.Produtos == null || !vendaDTO.Produtos.Any())
            {
                return BadRequest("A venda deve conter pelo menos um produto.");
            }

            // Busca os produtos no banco de dados pelos IDs fornecidos
            var produtoIds = vendaDTO.Produtos.Select(p => p.ProdutoId).ToList();
            var produtos = _context.Produtos.Where(p => produtoIds.Contains(p.Id)).ToList();

            if (produtos.Count != vendaDTO.Produtos.Count)
            {
                return BadRequest("Um ou mais produtos não foram encontrados.");
            }

            // Cria a venda
            var venda = new Venda
            {
                UsuarioId = vendaDTO.UsuarioId,
                Cliente = vendaDTO.Cliente,
                Pagamento = vendaDTO.Pagamento,
                DataVenda = DateTime.Now,
                Status = StatusVenda.Pendente
            };

            // Cria os itens da venda e calcula o total
            decimal total = 0;
            
            foreach (var produtoDTO in vendaDTO.Produtos)
            {
                var produto = produtos.First(p => p.Id == produtoDTO.ProdutoId);
                var itemVenda = new ItemVenda
                {
                    ProdutoId = produto.Id,
                    Venda = venda,
                    Quantidade = produtoDTO.Quantidade,
                    PrecoUnitario = produto.Preco
                };

                // Calcula o valor total para este item
                total += itemVenda.PrecoUnitario * itemVenda.Quantidade;

                venda.ItensVenda.Add(itemVenda);
            }

            // Define o total da venda
            venda.Total = total;

            _context.Vendas.Add(venda);
            _context.SaveChanges();

            return Ok(venda);
        }

        [HttpPut("{id}")]
        public ActionResult<Venda> Put(int id, [FromBody] Venda venda)
        {
            if (id != venda.Id)
            {
                return BadRequest();
            }

            _context.Entry(venda).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult<Venda> Delete(int id)
        {
            var venda = _context.Vendas.Find(id);

            if (venda == null)
            {
                return NotFound();
            }

            _context.Vendas.Remove(venda);
            _context.SaveChanges();

            return venda;
        }
    }
}