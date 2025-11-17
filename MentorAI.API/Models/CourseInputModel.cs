using System.ComponentModel.DataAnnotations;

namespace MentorAI.API.Models;

public class CourseInputModel
{
    [Required(ErrorMessage = "O título do curso é obrigatório.")]
    [StringLength(150)]
    public required string Titulo { get; set; }

    [StringLength(1000)]
    public string? Descricao { get; set; }

    [StringLength(100)]
    public string? Provedor { get; set; }

    [Required(ErrorMessage = "A carga horária é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "A carga horária deve ser maior que zero.")]
    public int CargaHoraria { get; set; }

    [Required(ErrorMessage = "O ID da skill é obrigatório.")]
    public required Guid SkillId { get; set; }
}