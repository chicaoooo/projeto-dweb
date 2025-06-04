using System.ComponentModel.DataAnnotations;

namespace DIEARD.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "O nome de utilizador é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome de utilizador não pode exceder 100 caracteres")]
        [Display(Name = "Nome de Utilizador")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "O número de telefone não pode exceder 15 caracteres")]
        [Display(Name = "Número de Telefone")]
        public string? PhoneNumber { get; set; }


        [Display(Name = "Foto do Perfil")]
        public IFormFile? FotoPerfilFile { get; set; }

        // Para mostrar a foto atual (Base64)
        public string? FotoPerfil { get; set; }

        // Flag para remover foto
        public bool RemoverFoto { get; set; }
    }
}