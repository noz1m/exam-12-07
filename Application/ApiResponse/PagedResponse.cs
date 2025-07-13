using System.Net;
namespace Application.ApiResponse;

public class PagedResponse<T> : Response<T>
{
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public PagedResponse(T? data, int totalRecords, int pageNumber, int pageSize) : base(data)
    {
        TotalRecords = totalRecords;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
    }

    public PagedResponse(string message, HttpStatusCode statusCode) : base(message, statusCode)
    {

    }
}
