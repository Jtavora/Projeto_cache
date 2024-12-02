using Microsoft.AspNetCore.Mvc;
using E_Commerce.Models;
using E_Commerce.DTOsCustom;
using E_Commerce.Interfaces;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendaController : ControllerBase
    {
        private readonly IVendaService _vendaService;

        public VendaController(IVendaService vendaService)
        {
            _vendaService = vendaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venda>>> Get()
        {
            var vendas = await _vendaService.GetAllVendasAsync();
            if (!vendas.Any())
            {
                return NotFound();
            }
            return Ok(vendas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Venda>> Get(int id)
        {
            var venda = await _vendaService.GetVendaByIdAsync(id);
            if (venda == null)
            {
                return NotFound();
            }
            return Ok(venda);
        }

        [HttpPost]
        public async Task<ActionResult<Venda>> Post([FromBody] VendaDTO vendaDTO)
        {
            try
            {
                var venda = await _vendaService.CreateVendaAsync(vendaDTO);
                return CreatedAtAction(nameof(Get), new { id = venda.Id }, venda);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] VendaDTO venda)
        {
            try
            {
                var vendaUpdated = await _vendaService.UpdateVendaAsync(id, venda);
                return Ok(vendaUpdated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _vendaService.DeleteVendaAsync(id))
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}