using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LojaGamerApi.Entities
{
    /// <summary>
    /// Entidade que representa um Usuário (Cliente ou Admin)
    /// </summary>
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        public string SenhaHash { get; set; } 

        [Required]
        public string Role { get; set; } // "Cliente" ou "Administrador"

        // Relacionamento: Um usuário pode ter vários pedidos
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}