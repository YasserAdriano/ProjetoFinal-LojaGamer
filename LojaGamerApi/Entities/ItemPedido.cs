using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaGamerApi.Entities
{
    /// <summary>
    /// Entidade que liga os Pedidos aos Produtos.
    /// </summary>
    public class ItemPedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; } // Chave estrangeira
        public Pedido Pedido { get; set; }

        [Required]
        public int ProdutoId { get; set; } // Chave estrangeira
        public Produto Produto { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PrecoUnitario { get; set; } // Guarda o preço do produto no momento da compra
    }
}