using Microsoft.EntityFrameworkCore;
using Biblioteca.API.Models;

namespace Biblioteca.API.Data
{
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options)
        {
            
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Livro> Livros { get; set; }
    }
}
