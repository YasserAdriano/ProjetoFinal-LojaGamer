using LojaGamerApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LojaGamerApi.Data
{

    public class LojaGamerContext : DbContext
    {
        public LojaGamerContext(DbContextOptions<LojaGamerContext> options) : base(options)
        {
        }

        
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }

    }
}