using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIEARD.Models
{
    /// <summary>
    /// utilizadores não anónimos da aplicação
    /// </summary>
    public class Utilizadores
    {
        /// <summary>
        /// identificador único do utilizador
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// nome do utilizador
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        public string Nome { get; set; }

        /// <summary>
        /// email do utilizador
        /// </summary>
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// número de telemóvel do utilizador
        /// </summary>
        [Display(Name = "Telemóvel")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
        [RegularExpression("([+]|00)?[0-9]{6,17}", ErrorMessage = "O {0} só pode conter digitos. No mínimo 6.")]
        public string Telemovel { get; set; }

        /// <summary>
        /// Morada do utilizador
        /// </summary>
        public string? Morada { get; set; }

        /// <summary>
        /// Código Postal da morada do utilizador
        /// </summary>
        [Display(Name = "Código Postal")]
        [RegularExpression("[1-9][0-9]{3}[-|\\s][0-9]{3}", ErrorMessage = "O {0} tem de seguir o formato xxxx-xxx")]
        public string? CodPostal { get; set; }

        /// <summary>
        /// ID do utilizador no sistema Identity
        /// </summary>
        [Required]
        public string UserId { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }
    }
}