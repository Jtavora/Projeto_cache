using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Models
{
    public enum StatusVenda
    {
        Pendente,
        Concluida,
        Cancelada
    }

    [Table("Vendas")]
    public class Venda
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public int UsuarioId { get; set; }  // Vendedor

        [Column(TypeName = "varchar(100)")]
        public string Cliente { get; set; }  // Cliente

        [Column(TypeName = "datetime")]
        public DateTime DataVenda { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Pagamento { get; set; } // Relacionamento com Pagamento

        [Column(TypeName = "varchar(100)")]
        public StatusVenda Status { get; set; }  // Ex: "Pendente", "Concluída", etc.

        // Propriedade de navegação inversa
        public virtual ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();

        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}