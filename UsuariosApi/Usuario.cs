// ============================================================
// Usuario.cs
// Modelo de dados: define quais campos um usuário tem.
// É o formato que a API recebe e devolve em JSON.
//
// Exemplo de JSON (note que o C# converte para camelCase):
//   { "id": 1, "nome": "Rafael", "email": "rafael@email.com" }
// ============================================================
public class Usuario
{
    // Identificador único. Quem gera é o SERVIDOR (no POST),
    // então quem consome a API não precisa enviar o id ao criar.
    public int Id { get; set; }

    // Nome do usuário (obrigatório)
    public string Nome { get; set; } = "";

    // E-mail do usuário (obrigatório, precisa ser único)
    public string Email { get; set; } = "";
}