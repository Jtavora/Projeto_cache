using E_Commerce.Models;
using E_Commerce.DTOsCustom;

namespace E_Commerce.Interfaces
{
    public interface IVendaService
    {
        Task<IEnumerable<Venda>> GetAllVendasAsync();
        Task<Venda> GetVendaByIdAsync(int id);
        Task<Venda> CreateVendaAsync(VendaDTO vendaDTO);
        Task<bool> UpdateVendaAsync(int id, Venda venda);
        Task<bool> DeleteVendaAsync(int id);
    }
}