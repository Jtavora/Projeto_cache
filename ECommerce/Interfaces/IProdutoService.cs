using E_Commerce.Models;

namespace E_Commerce.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<Produto>> GetAllProdutosAsync();
        Task<Produto> GetProdutoByIdAsync(int id);
        Task<Produto> CreateProdutoAsync(Produto produto);
        Task<Produto> UpdateProdutoAsync(int id, Produto produto);
        Task<Produto> DeleteProdutoAsync(int id);
    }
}