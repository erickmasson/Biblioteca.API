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

        #region CreateLivroAsync
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
        #endregion
        #region DeleteLivroAsync
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
        #endregion
        #region GetLivroPorIdAsync e GetTodosLivrosAsync
        public async Task<Livro?> GetLivroPorIdAsync(int id)
        {
            return await _context.Livros.FindAsync(id);
        }

        public async Task<PagedResult<Livro>> GetTodosLivrosAsync(int pageNumber, int pageSize, string? searchQuery, string? genero)
        {
            IQueryable<Livro> query = _context.Livros.AsQueryable();

            if(!string.IsNullOrEmpty(searchQuery))
            {
                var lowerCaseSearchQuery = searchQuery.ToLower();
                query = query.Where(l=>l.Titulo.ToLower().Contains(lowerCaseSearchQuery) || 
                                       l.Autor.ToLower().Contains(lowerCaseSearchQuery) 
                );
            }

            if (!string.IsNullOrEmpty(genero))
            {
                query = query.Where(l=>l.Genero.ToLower() == genero.ToLower());
            }

            var totalCount = await query.CountAsync();

            var items = await query.OrderBy(l => l.Titulo).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<Livro>(items, pageNumber, pageSize, totalCount);
        }
        #endregion
        #region GetPdfStreamAsync
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
        #endregion
        #region UpdateLivroAsync
        public async Task<bool> UpdateLivroAsync(int id, UpdateLivroDto livroDto)
        {
            var livro = await _context.Livros.FindAsync(id);
            if (livro == null)
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
        #endregion
    }
}
