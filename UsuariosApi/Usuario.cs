// "Ficha em branco": define quais campos um usuário tem.
// Só os títulos dos campos, sem valores.
public class Usuario
{
    // Número inteiro que identifica o usuário (1, 2, 3...)
    // { get; set; } = pode ser lido e alterado
    public int Id { get; set; }

    // Texto. O = "" dá um valor inicial vazio, para nunca começar como null
    public string Nome { get; set; } = "";

    // Mesma ideia do Nome
    public string Email { get; set; } = "";
}