using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_Commerce.Models
{
    [Table("ItensVenda")]
    public class ItemVenda
    {
        public int Id { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }

        [ForeignKey("Produto")]
        public int ProdutoId { get; set; }

        
        public virtual Produto Produto { get; set; }
        
        [ForeignKey("Venda")]
        public int VendaId { get; set; }

        [JsonIgnore]
        public virtual Venda Venda { get; set; } 
    }
}