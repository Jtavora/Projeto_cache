using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }  // Vendedor
        public int ClienteId { get; set; }  // Cliente
        public DateTime DataVenda { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }  // Ex: "Pendente", "Concluída", etc.
        public ICollection<ItemVenda> ItensVenda { get; set; } // Relacionamento com ItemVenda
        public Pagamento Pagamento { get; set; } // Relacionamento com Pagamento
    }
}