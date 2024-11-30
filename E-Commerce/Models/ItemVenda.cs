using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Models
{
    public class ItemVenda
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int VendaId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public Produto Produto { get; set; }  // Relacionamento com Produto
        public Venda Venda { get; set; }  // Relacionamento com Venda
    }
}