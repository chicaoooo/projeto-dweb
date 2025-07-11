using System.ComponentModel.DataAnnotations;

namespace DIEARD.Models.ApiModels;

public class LoginApiModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}