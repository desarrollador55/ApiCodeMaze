using ApiCodeMaze.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ApiCodeMaze.Repositorio.Contexts
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options): base(options){}

        public DbSet<Owner> Owners { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}
