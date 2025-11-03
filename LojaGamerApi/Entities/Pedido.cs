using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace LojaGamerApi.Entities
{
    /// <summary>
    /// Entidade que representa um Pedido (compra finalizada).
    /// </summary>
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; } // Chave estrangeira
        public Usuario Usuario { get; set; }

        [Required]
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ValorTotal { get; set; }

        [Required]
        public string Status { get; set; } // Ex: "Pendente", "Pago", "Enviado", "Cancelado"

        // Relacionamento: Um pedido é composto de vários itens
        public ICollection<ItemPedido> ItensDoPedido { get; set; } = new List<ItemPedido>();
    }
}