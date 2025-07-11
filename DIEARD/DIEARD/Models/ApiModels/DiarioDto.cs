using System.ComponentModel.DataAnnotations;

namespace DIEARD.Models.ApiModels;

public class DiarioDto
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Conteudo { get; set; }
    public string Categoria { get; set; }
    public string Autor { get; set; }
    public DateTime DataCriacao { get; set; }
    public int? MoodTracker { get; set; }
}

public class DiarioCreateDto
{
    [Required]
    [StringLength(200)]
    public string Titulo { get; set; }

    [Required]
    public string Conteudo { get; set; }

    [Required]
    public int CategoriaId { get; set; }

    [Range(0, 100)]
    public int? MoodTracker { get; set; }
}

public class DiarioUpdateDto
{
    [Required]
    [StringLength(200)]
    public string Titulo { get; set; }

    [Required]
    public string Conteudo { get; set; }

    [Required]
    public int CategoriaId { get; set; }

    [Range(0, 100)]
    public int? MoodTracker { get; set; }
}