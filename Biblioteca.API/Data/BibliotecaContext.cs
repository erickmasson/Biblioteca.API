using Microsoft.EntityFrameworkCore;
using Biblioteca.API.Models;

namespace Biblioteca.API.Data
{
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options)
        {
            
        }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Livro> Livros { get; set; }
    }
}
