using System;
using System.Collections.Generic;

namespace MentorAI.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string CargoAtual { get; private set; }
        public string CargoDesejado { get; private set; }
        
        public ICollection<Course> CursosAtivos { get; private set; }
        
        public User(string nome, string email, string cargoAtual, string cargoDesejado)
        {
            Nome = nome;
            Email = email;
            CargoAtual = cargoAtual;
            CargoDesejado = cargoDesejado;
            CursosAtivos = new List<Course>();
        }

        public void Refresh(string nome, string email, string cargoAtual, string cargoDesejado)
        {
            Nome = nome;
            Email = email;
            CargoAtual = cargoAtual;
            CargoDesejado = cargoDesejado;
        }

        public record UserResponse(
            Guid Id,
            string Nome,
            string Email,
            string CargoAtual,
            string CargoDesejado
        );
    }
}