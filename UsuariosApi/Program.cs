// ============================================================
// Program.cs - API de CRUD de usuários (ASP.NET Core Minimal API)
//
// COMO RODAR
//   dotnet run          (ou "dotnet watch" para reiniciar ao salvar)
//   A porta aparece no terminal na linha "Now listening on: ..."
//
// RESUMO DOS ENDPOINTS (base: http://localhost:<porta>)
//
//   GET    /usuarios        -> lista todos            -> 200 + array de usuários
//   GET    /usuarios/{id}   -> busca um               -> 200 + usuário | 404
//   POST   /usuarios        -> cria um                -> 201 + usuário criado | 400 | 409
//   PUT    /usuarios/{id}   -> atualiza um            -> 200 + usuário atualizado | 400 | 404 | 409
//   DELETE /usuarios/{id}   -> apaga um               -> 204 (sem corpo) | 404
//
// FORMATO DO JSON (POST e PUT)
//   Header obrigatório: Content-Type: application/json
//   Corpo: { "nome": "Fulano", "email": "fulano@email.com" }
//   - NÃO precisa enviar "id": o servidor gera no POST e usa o da URL no PUT.
//   - Os nomes dos campos vão em minúsculo (camelCase): id, nome, email.
//
// STATUS CODES USADOS
//   200 OK            -> deu certo, devolvendo dados
//   201 Created       -> usuário criado
//   204 No Content    -> apagado com sucesso (sem corpo na resposta)
//   400 Bad Request   -> dados inválidos (nome vazio, e-mail sem "@")
//   404 Not Found     -> não existe usuário com esse id
//   409 Conflict      -> já existe outro usuário com esse e-mail
//
// ATENÇÃO
//   Os dados ficam só em MEMÓRIA: quando o servidor reinicia, tudo volta
//   para os 2 usuários iniciais. Para persistir, o próximo passo é
//   trocar a lista por um banco (Entity Framework + SQLite/MySQL).
// ============================================================

// ---------- 1) CONFIGURAÇÃO ----------
// O "builder" prepara a aplicação (serviços e configurações).
var builder = WebApplication.CreateBuilder(args);

// CORS: sem isso, um front-end rodando em outra porta/endereço
// (ex.: React em localhost:3000) é BLOQUEADO pelo navegador.
// Esta política libera tudo, o que é ok para desenvolvimento/estudo.
// Em produção, restrinja às origens permitidas (WithOrigins(...)).
builder.Services.AddCors(opcoes =>
    opcoes.AddDefaultPolicy(politica =>
        politica.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()));

// ---------- 2) CONSTRUÇÃO ----------
// Depois do Build(), não registramos mais serviços: só definimos rotas.
var app = builder.Build();

// Ativa a política de CORS definida acima (precisa vir antes das rotas).
app.UseCors();

// ---------- 3) DADOS (em memória) ----------
// Lista que faz o papel de "banco de dados" por enquanto.
// Começa com 2 usuários de exemplo para facilitar os testes.
var usuarios = new List<Usuario>
{
    new Usuario { Id = 1, Nome = "Rafael", Email = "rafael@email.com" },
    new Usuario { Id = 2, Nome = "Bianca", Email = "bianca@email.com" }
};

// Contador do próximo Id. Começa em 3 porque 1 e 2 já existem.
// É incrementado a cada usuário criado, para nunca repetir Id,
// mesmo depois de apagar usuários.
var proximoId = 3;

// ---------- 4) ROTAS ----------

// READ (todos): GET /usuarios
// Devolve a lista inteira. O C# converte para JSON automaticamente.
app.MapGet("/usuarios", () => usuarios);

// READ (um): GET /usuarios/{id}
// O {id} da URL chega no parâmetro "int id".
app.MapGet("/usuarios/{id}", (int id) =>
{
    // FirstOrDefault devolve o primeiro usuário com esse Id, ou null se não achar.
    var usuario = usuarios.FirstOrDefault(u => u.Id == id);

    // Não achou -> 404
    if (usuario is null)
    {
        return Results.NotFound();
    }

    // Achou -> 200 com o usuário em JSON
    return Results.Ok(usuario);
});

// CREATE: POST /usuarios
// O JSON enviado no corpo vira o objeto "novo" automaticamente.
app.MapPost("/usuarios", (Usuario novo) =>
{
    // Validação: nome e e-mail não podem ser vazios, e o e-mail precisa ter "@".
    if (string.IsNullOrWhiteSpace(novo.Nome) || string.IsNullOrWhiteSpace(novo.Email))
    {
        return Results.BadRequest("Nome e Email são obrigatórios.");
    }
    if (!novo.Email.Contains('@'))
    {
        return Results.BadRequest("Email inválido.");
    }

    // E-mail único: compara ignorando maiúsculas/minúsculas.
    var emailJaExiste = usuarios.Any(u =>
        u.Email.Equals(novo.Email, StringComparison.OrdinalIgnoreCase));
    if (emailJaExiste)
    {
        return Results.Conflict("Já existe um usuário com esse e-mail.");
    }

    // O servidor escolhe o Id (ignora qualquer id enviado pelo cliente).
    novo.Id = proximoId;
    proximoId++;

    usuarios.Add(novo);

    // 201 Created. O primeiro argumento é o endereço do recurso criado.
    return Results.Created($"/usuarios/{novo.Id}", novo);
});

// UPDATE: PUT /usuarios/{id}
// Recebe o id na URL e os novos dados no corpo (nome e email).
app.MapPut("/usuarios/{id}", (int id, Usuario dados) =>
{
    var usuario = usuarios.FirstOrDefault(u => u.Id == id);

    if (usuario is null)
    {
        return Results.NotFound();
    }

    if (string.IsNullOrWhiteSpace(dados.Nome) || string.IsNullOrWhiteSpace(dados.Email))
    {
        return Results.BadRequest("Nome e Email são obrigatórios.");
    }
    if (!dados.Email.Contains('@'))
    {
        return Results.BadRequest("Email inválido.");
    }

    // E-mail único, mas permitindo manter o próprio e-mail (u.Id != id).
    var emailEmUsoPorOutro = usuarios.Any(u =>
        u.Id != id &&
        u.Email.Equals(dados.Email, StringComparison.OrdinalIgnoreCase));
    if (emailEmUsoPorOutro)
    {
        return Results.Conflict("Já existe outro usuário com esse e-mail.");
    }

    // Atualiza os campos do usuário que já está na lista.
    // O Id nunca muda.
    usuario.Nome = dados.Nome;
    usuario.Email = dados.Email;

    return Results.Ok(usuario);
});

// DELETE: DELETE /usuarios/{id}
app.MapDelete("/usuarios/{id}", (int id) =>
{
    var usuario = usuarios.FirstOrDefault(u => u.Id == id);

    if (usuario is null)
    {
        return Results.NotFound();
    }

    usuarios.Remove(usuario);

    // 204: deu certo, e não há nada para devolver.
    return Results.NoContent();
});

// ---------- 5) EXECUÇÃO ----------
// Liga o servidor e fica esperando pedidos. Precisa ser a última linha.
app.Run();