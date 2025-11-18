
# 📘 MentorAI — Plataforma de Recomendação e Gestão de Aprendizado  
### *Global Solution – FIAP | 2025*

---

## 👥 **Integrantes do Grupo**
| Nome | RM |
|------|------|
| **Caetano Matos Penafiel** | **557984** |
| **Kauã Fermino Zipf** | **558957** |
| **Victor Egídio Lira** | **556653** |

---

## 🚀 **Descrição do Projeto**
O **MentorAI** é uma API desenvolvida em **.NET 8**, cujo objetivo é auxiliar colaboradores a identificarem cursos relevantes para sua evolução profissional.  
A aplicação mantém:

- **Usuários** com informações de carreira atual e desejada  
- **Skills (competências)** que podem ser desenvolvidas  
- **Cursos** associados a uma skill  
- Regras de **matrícula dos usuários em cursos**
- **Paginação**, **HATEOAS**, **CRUD completo**, **testes unitários** e **banco Oracle**

O projeto segue boas práticas de arquitetura em camadas e padrões como *Repository Pattern*, *Use Cases*, *DTOs (Input/Response)* e *Entity Mapping*.

---

## 🏗 **Arquitetura da Solução**

```
MentorAI
 ├── MentorAI.API               → Controllers, InputModels, Configurações
 ├── MentorAI.Application       → UseCases, Regras de Negócio
 ├── MentorAI.Domain            → Entidades, Interfaces, Pagination
 ├── MentorAI.Infrastructure    → EF Core, Repositories, Mappings, Migrations
 └── MentorAI.Tests             → Testes unitários (xUnit + Moq)
```

---

## 🧩 **Entidades Principais**

### 🧑‍💼 **User**
- Nome, Email  
- Cargo Atual  
- Cargo Desejado  
- Cursos Ativos (relação N-N)

### 🎓 **Course**
- Título, Descrição  
- Provedor  
- Carga Horária  
- Skill associada  
- Usuários matriculados (N-N)

### 🧠 **Skill**
- Nome  
- Descrição  
- Cursos associados (1-N)

---

## 🔗 **Relacionamentos**
- **1 Skill → N Cursos**
- **N Usuários ↔ N Cursos**

A tabela de junção `USUARIOS_CURSOS` é construída automaticamente pelo EF Core.

---

## 📡 **Principais Endpoints**

### 🔹 **UserController**
| Método | Rota | Descrição |
|-------|------|-----------|
| GET | `/User` | Lista todos os usuários |
| GET | `/User/{id}` | Busca por ID |
| POST | `/User` | Cria um usuário |
| PUT | `/User/{id}` | Atualiza |
| DELETE | `/User/{id}` | Remove |
| GET | `/User/paginado` | Paginação + HATEOAS |
| POST | `/User/{userId}/courses/{courseId}` | Matricula o usuário no curso |

---

### 🔹 **CourseController**
| Método | Rota | Descrição |
|-------|------|-----------|
| GET | `/Course` | Lista cursos com relações (Skill + Usuários) |
| GET | `/Course/{id}` | Busca por ID com relações |
| POST | `/Course` | Cria um curso |
| PUT | `/Course/{id}` | Atualiza |
| DELETE | `/Course/{id}` | Remove |
| GET | `/Course/paginado` | Paginação + HATEOAS |

---

### 🔹 **SkillController**
| Método | Rota | Descrição |
|-------|------|-----------|
| GET | `/Skill` | Lista todas |
| GET | `/Skill/{id}` | Busca por ID |
| POST | `/Skill` | Cria |
| PUT | `/Skill/{id}` | Atualiza |
| DELETE | `/Skill/{id}` | Remove |
| GET | `/Skill/paginado` | Paginação + HATEOAS |

---

## 🛠 **Tecnologias Utilizadas**

- **.NET 8**
- **Entity Framework Core**
- **Oracle Database**
- **Swagger / OpenAPI**
- **xUnit**
- **Moq**
- **FluentAssertions**
- **Clean Architecture / Separation of Concerns**

---

## 🧪 **Testes Automatizados**

O projeto inclui testes unitários para:

- **UserController**
- **CourseController**
- **SkillController**
- **UserCourseUseCase**
- **Repositories (via InMemoryDatabase)**

Frameworks utilizados:

- `xUnit`
- `Moq`
- `FluentAssertions`
- `EF Core InMemory`

Para executar:

```
dotnet test
```

---

## ▶️ **Como Executar o Projeto**

### 1. Restaurar dependências
```
dotnet restore
```

### 2. Aplicar migrations (caso necessário)
```
dotnet ef database update --project MentorAI.Infrastructure --startup-project MentorAI.API
```

### 3. Rodar API
```
dotnet run --project MentorAI.API
```

Acesse o Swagger:

👉 http://localhost:5000/swagger

---

## 🧭 **Pontos Técnicos Atendidos (GS)**

✔ CRUD completo das entidades  
✔ Padrão Repository  
✔ Padrão UseCase  
✔ Paginação e HATEOAS  
✔ DTOs de entrada e saída  
✔ Banco Oracle + Migrations  
✔ Testes unitários  
✔ Injeção de Dependência  
✔ Separação completa de camadas  
✔ Documentação com Swagger  
✔ Código padronizado e limpo  

---

## 📄 **Licença**
Projeto acadêmico desenvolvido para a **FIAP — Global Solution 2025**.
