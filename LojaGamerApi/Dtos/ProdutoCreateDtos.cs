using System.ComponentModel.DataAnnotations;

namespace LojaGamerApi.Dtos
{
    // Define os dados que o front-end DEVE enviar para criar um produto
    public class ProdutoCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Required]
        [Range(0.01, 100000.00)]
        public decimal Preco { get; set; }

        [Required]
        [Range(0, 9999)]
        public int Estoque { get; set; }
    }
}