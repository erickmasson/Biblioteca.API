using Biblioteca.API.Data;
using Biblioteca.API.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.API.Services
{
    public class LivroService : ILivroService
    {
        private readonly BibliotecaContext _context;

        public LivroService(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<Livro> CreateLivroAsync(CreateLivroDto livroDto)
        {
            if (livroDto == null || livroDto.ArquivoPdf.Length == 0)
            {
                throw new ArgumentException("O arquivo pdf é obrigatório.");
            }

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Pdfs");

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var nomeArquivoUnico = Guid.NewGuid().ToString() + Path.GetExtension(livroDto.ArquivoPdf.FileName);
            var caminhoArquivo = Path.Combine(uploadsPath, nomeArquivoUnico);

            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await livroDto.ArquivoPdf.CopyToAsync(stream);
            }

            var livro = new Livro
            {
                Titulo = livroDto.Titulo,
                Autor = livroDto.Autor,
                Genero = livroDto.Genero,
                AnoPublicacao = livroDto.AnoPublicacao,
                CaminhoPdf = caminhoArquivo,
                DataUpload = DateTime.UtcNow
            };

            await _context.Livros.AddAsync(livro);
            await _context.SaveChangesAsync();

            return livro;
        }

        public async Task<bool> DeleteLivroAsync(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            if (livro == null)
            {
                return false;
            }

            if (File.Exists(livro.CaminhoPdf))
            {
                File.Delete(livro.CaminhoPdf);
            }
            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Livro?> GetLivroPorIdAsync(int id)
        {
            return await _context.Livros.FindAsync(id);
        }

        public async Task<IEnumerable<Livro>> GetTodosLivrosAsync()
        {
            return await _context.Livros.ToListAsync();
        }

        public async Task<(Stream stream, string contentType, string fileName)?> GetPdfStreamAsync(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            if (livro == null || string.IsNullOrEmpty(livro.CaminhoPdf) || !File.Exists(livro.CaminhoPdf))
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(livro.CaminhoPdf, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;
            var nomeArquivo = Path.GetFileName(livro.CaminhoPdf);
            return (memoryStream, "application/pdf", nomeArquivo);
        }

        public async Task<bool> UpdateLivroAsync(int id, UpdateLivroDto livroDto)
        {
            var livro = await _context.Livros.FindAsync(id);
            if(livro == null)
            {
                return false;
            }

            livro.Titulo = livroDto.Titulo;
            livro.Autor = livroDto.Autor;
            livro.Genero = livroDto.Genero;
            livro.AnoPublicacao = livroDto.AnoPublicacao;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
