using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Models
{
    public class Pagamento
    {
        public int Id { get; set; }
        public int VendaId { get; set; }
        public decimal Valor { get; set; }
        public string Metodo { get; set; }  // Ex: "Cartão de Crédito", "Boleto", etc.
        public string Status { get; set; }  // Ex: "Pendente", "Aprovado", "Rejeitado"
        public Venda Venda { get; set; }  // Relacionamento com Venda
    }
}