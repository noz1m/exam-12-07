using Application.ApiResponse;
using Microsoft.EntityFrameworkCore;
namespace Application.Paginations;

public class Pagination<T>(IQueryable<T> queryable)
{
    public async Task<PagedResponse<List<T>>> GetPagedResponseAsync(int pageNumber, int pageSize)
    {
        try
        {
            var validFilter = new ValidFilter(pageNumber, pageSize);
            var query = queryable.AsQueryable();
            var totalCount = await query.CountAsync();
            var data = await query
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();

            return new PagedResponse<List<T>>(data, totalCount, validFilter.PageNumber, validFilter.PageSize);
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}
