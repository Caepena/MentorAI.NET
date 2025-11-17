using System;
using System.Collections.Generic;

namespace MentorAI.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; }
        public string? Descricao { get; private set; }
        public string? Provedor { get; private set; }
        public int CargaHoraria { get; private set; }
        
        public Guid SkillId { get; private set; }
        public Skill? Skill { get; private set; }
        
        public ICollection<User> UsuariosMatriculados { get; private set; }

        public Course(
            string titulo,
            string? descricao,
            string? provedor,
            int cargaHoraria,
            Guid skillId)
        {
            Titulo = titulo;
            Descricao = descricao;
            Provedor = provedor;
            CargaHoraria = cargaHoraria;
            SkillId = skillId;

            UsuariosMatriculados = new List<User>();
        }

        public void Refresh(
            string titulo,
            string? descricao,
            string? provedor,
            int cargaHoraria)
        {
            Titulo = titulo;
            Descricao = descricao;
            Provedor = provedor;
            CargaHoraria = cargaHoraria;
        }

        public record CursoResponse(
            Guid Id,
            string Titulo,
            string? Descricao,
            string? Provedor,
            int CargaHoraria,
            Guid SkillId
        );
    }
}