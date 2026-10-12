var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var usuarios = new List<Usuario>
{
    new Usuario { Id = 1, Nome = "Rafael", Email = "rafael@email.com" },
    new Usuario { Id = 2, Nome = "Bianca", Email = "bianca@email.com" }
};

app.MapGet("/ping", () => "pong");
app.MapGet("/usuarios", () => usuarios);

app.Run();