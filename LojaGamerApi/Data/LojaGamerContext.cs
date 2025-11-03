using LojaGamerApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LojaGamerApi.Data
{
    // Esta classe é a nossa ponte entre as classes C# (Entities) e o banco de dados.
    // Ela herda de DbContext, que é a classe principal do Entity Framework.
    public class LojaGamerContext : DbContext
    {
        // O construtor, que recebe as configurações de conexão
        public LojaGamerContext(DbContextOptions<LojaGamerContext> options) : base(options)
        {
        }

        // Estas linhas dizem ao Entity Framework quais classes devem virar tabelas no banco.
        // Se você não colocar uma classe aqui, o EF não vai saber dela.
        
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }

    }
}