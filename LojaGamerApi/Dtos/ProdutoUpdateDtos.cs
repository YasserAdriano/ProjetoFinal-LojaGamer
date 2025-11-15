using System.ComponentModel.DataAnnotations;

namespace LojaGamerApi.Dtos
{
    public class ProdutoUpdateDto
    {
        [StringLength(200)]
        public string? Nome { get; set; }

        public string? Descricao { get; set; }

        [Range(0.01, 100000.00)]
        public decimal? Preco { get; set; }

        [Range(0, 9999)]
        public int? Estoque { get; set; }
    }
}