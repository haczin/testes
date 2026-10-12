// 1) CONFIGURAÇÃO: prepara a aplicação web
var builder = WebApplication.CreateBuilder(args);

// 2) CONSTRUÇÃO: cria a aplicação a partir da configuração.
// Depois dessa linha, só definimos rotas.
var app = builder.Build();

// 3) DADOS: a lista de usuários (a "pilha de fichas preenchidas").
// Precisa vir ANTES das rotas, porque o C# lê de cima para baixo.
// Cada "new Usuario { ... }" é uma ficha preenchida.
// O ; aparece só uma vez, depois do } final.
var usuarios = new List<Usuario>
{
    new Usuario { Id = 1, Nome = "Rafael", Email = "rafael@email.com" },
    new Usuario { Id = 2, Nome = "Bianca", Email = "bianca@email.com" }
};

// 4) ROTAS: o que a API responde a cada pedido.
// MapGet = pedidos do tipo GET (o que o navegador faz ao digitar uma URL)

// GET /ping -> responde o texto "pong" (rota de teste)
app.MapGet("/ping", () => "pong");

// GET /usuarios -> devolve a lista inteira.
// O C# converte a lista para JSON automaticamente.
app.MapGet("/usuarios", () => usuarios);
app.MapGet("/usuarios/{id}", (int id) =>
{
    var usuario = usuarios.FirstOrDefault(u => u.Id == id);

    if (usuario is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(usuario);
});
// 5) EXECUÇÃO: liga o servidor e fica esperando pedidos.
// Tem que ser a última linha.
app.Run();