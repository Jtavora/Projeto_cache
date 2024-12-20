using Microsoft.AspNetCore.Mvc;
using E_Commerce.Interfaces;
using E_Commerce.Models;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            var produtos = await _produtoService.GetAllProdutosAsync();
            if (produtos == null || !produtos.Any())
            {
                return NotFound();
            }
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> Get(int id)
        {
            var produto = await _produtoService.GetProdutoByIdAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> Post([FromBody] Produto produto)
        {
            var createdProduto = await _produtoService.CreateProdutoAsync(produto);
            return CreatedAtAction(nameof(Get), new { id = createdProduto.Id }, createdProduto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Produto>> Put(int id, [FromBody] Produto produto)
        {
            var updatedProduto = await _produtoService.UpdateProdutoAsync(id, produto);
            if (updatedProduto == null)
            {
                return NotFound();
            }
            return Ok(updatedProduto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deletedProduto = await _produtoService.DeleteProdutoAsync(id);

            if (deletedProduto == null)
            {
                return NotFound();
            }

            return Ok(deletedProduto);
        }
    }
}