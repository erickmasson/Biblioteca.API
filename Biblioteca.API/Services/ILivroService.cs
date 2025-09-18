using Biblioteca.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Services
{
    public interface ILivroService
    {
        Task<PagedResult<Livro>> GetTodosLivrosAsync(int pageNumber, int pageSize);
        Task<Livro>? GetLivroPorIdAsync(int id);
        Task<Livro> CreateLivroAsync(CreateLivroDto livroDto);
        Task<bool> UpdateLivroAsync(int id, UpdateLivroDto livroDto);
        Task<bool> DeleteLivroAsync(int id);
        Task<(Stream stream, string contentType, string fileName)?> GetPdfStreamAsync(int id);
    }
}
