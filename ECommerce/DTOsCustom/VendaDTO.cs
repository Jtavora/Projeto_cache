using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.DTOsCustom
{
    public class ProdutoQuantidadeDTO
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }

    public class VendaDTO
    {
        public int UsuarioId { get; set; }
        public string Cliente { get; set; }
        public string Pagamento { get; set; }
        public List<ProdutoQuantidadeDTO> Produtos { get; set; } = new List<ProdutoQuantidadeDTO>();
    }
}