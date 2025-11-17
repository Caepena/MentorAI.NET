using System;
using System.Collections.Generic;

namespace MentorAI.Domain.Entities
{
    public class Skill
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string? Descricao { get; private set; }


        public ICollection<Course> Cursos { get; private set; }

        public Skill(string nome, string? descricao)
        {
            Nome = nome;
            Descricao = descricao;
            Cursos = new List<Course>();
        }

        public void Refresh(string nome, string? descricao)
        {
            Nome = nome;
            Descricao = descricao;
        }

        public record SkillResponse(
            Guid Id,
            string Nome,
            string? Descricao
        );
    }
}