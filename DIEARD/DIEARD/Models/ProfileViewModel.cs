using System.ComponentModel.DataAnnotations;

namespace DIEARD.Models
{
    

    public class ProfileViewModel
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Número de telefone inválido")]
        public string? PhoneNumber { get; set; }
    }
}
