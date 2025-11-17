using System.ComponentModel.DataAnnotations;

namespace MentorAI.API.Models;

public class UserInputModel
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public required string Nome { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(150)]
    public required string Email { get; set; }

    [Required(ErrorMessage = "O cargo atual é obrigatório.")]
    [StringLength(100)]
    public required string CargoAtual { get; set; }

    [Required(ErrorMessage = "O cargo desejado é obrigatório.")]
    [StringLength(100)]
    public required string CargoDesejado { get; set; }
}