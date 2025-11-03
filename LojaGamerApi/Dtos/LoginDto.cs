using System.ComponentModel.DataAnnotations;

namespace LojaGamerApi.Dtos
{
    // Esta classe define os dados que o front-end DEVE enviar para fazer login
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}