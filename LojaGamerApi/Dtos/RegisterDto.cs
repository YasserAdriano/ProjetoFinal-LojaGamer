using System.ComponentModel.DataAnnotations;

namespace LojaGamerApi.Dtos
{
    // Esta classe define os dados que o front-end DEVE enviar para se registrar
    public class RegisterDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Senha { get; set; }
    }
}