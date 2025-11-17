using System.ComponentModel.DataAnnotations;

namespace MentorAI.API.Models;

public class SkillInputModel
{
    [Required(ErrorMessage = "O nome da habilidade é obrigatório.")]
    [StringLength(100)]
    public required string Nome { get; set; }

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Descricao { get; set; }
}