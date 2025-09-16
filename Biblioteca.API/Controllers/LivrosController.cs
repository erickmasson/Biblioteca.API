using Biblioteca.API.Data;
using Biblioteca.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

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

            if (livro == null)
            {
                return NotFound();
            }

            return Ok(livro);
        }

        [HttpPost]
        [Authorize]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
        public async Task<IActionResult> Postlivro([FromForm] CreateLivroDto livroDto) {
            if (livroDto == null || livroDto.ArquivoPdf.Length == 0)
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

            return CreatedAtAction(nameof(livro), new { id = livro.Id }, livro);
        }

        [HttpPut("{Id}")]
        [Authorize]
        public async Task<IActionResult> UpdateLivro(int id, UpdateLivroDto livroDto)
        {
            var livro = await _context.Livros.FindAsync(id);

            if(livro == null)
            {
                return NotFound();
            }

            livro.Titulo = livroDto.Titulo;
            livro.Autor = livroDto.Autor;
            livro.Genero = livroDto.Genero;
            livro.AnoPublicacao = livroDto.AnoPublicacao;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            if (livro == null)
            {
                return NotFound();
            }
            //TODO: Adicionar lógica para deletar o arquivo PDF físico do servidor
            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var livro = await _context.Livros.FindAsync(id);

            if(livro == null)
            {
                return NotFound("O registro do livro não foi encontrado.");
            }

            if(string.IsNullOrEmpty(livro.CaminhoPdf) || !System.IO.File.Exists(livro.CaminhoPdf))
            {
                return NotFound("O arquivo PDF não foi encontrado no servidor.");
            }

            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(livro.CaminhoPdf, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }

            memoryStream.Position = 0;

            var nomeArquivo = Path.GetFileName(livro.CaminhoPdf);
            return File(memoryStream, "application/pdf", nomeArquivo);
        }
    }
}
