using Biblioteca.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteca.API.Dtos;

namespace Biblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly BibliotecaContext _context;
        public LivrosController(BibliotecaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLivros()
        {
            var livros = await _context.Livros.ToListAsync();
            return Ok(livros);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            if(livro == null)
            {
                return NotFound();
            }

            return Ok(livro);
        }

        [HttpPost]
        public async Task<IActionResult> Postlivro([FromForm] CreateLivroDto livroDto) {
            if(livroDto == null || livroDto.ArquivoPdf.Length == 0)
            {
                return BadRequest("O arquivo pdf é obrigatório.");
            }

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Pdfs");

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var nomeArquivoUnico = Guid.NewGuid().ToString() + Path.GetExtension(livroDto.ArquivoPdf.FileName); 
            var caminhoArquivo = Path.Combine(uploadsPath, nomeArquivoUnico);

            using(var stream = new FileStream(caminhoArquivo, FileMode.Create))
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

            return CreatedAtAction(nameof(livro), new { id = livro.Id }, livro);
        }
    }
}
