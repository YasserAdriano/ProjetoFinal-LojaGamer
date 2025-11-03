using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LojaGamerApi.Entities
{
    /// <summary>
    /// Entidade que representa um Produto da loja.
    /// </summary>
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }

        [Required]
        public int Estoque { get; set; } // Usado para a RN02: "Produtos sem estoque não podem ser comprados"
    }
}