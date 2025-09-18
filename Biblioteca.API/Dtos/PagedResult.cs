namespace Biblioteca.API.Dtos
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } // A lista de itens da página atual
        public int PageNumber { get; set; } // O número da página atual
        public int PageSize { get; set; } // O tamanho da página
        public int TotalCount { get; set; } // O número total de itens no banco
        public int TotalPages { get; set; } // O número total de páginas

        public PagedResult(List<T> items, int pageNumber, int pageSize, int totalCount)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}