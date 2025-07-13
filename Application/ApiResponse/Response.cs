using System.Net;
namespace Application.ApiResponse;

public class Response<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public int StatusCode { get; set; }

    public Response(T? data, string? message = null)
    {
        IsSuccess = true;
        Message = message;
        Data = data;
        StatusCode = (int)HttpStatusCode.OK;
    }
    public Response(string message, HttpStatusCode statusCode)
    {
        IsSuccess = false;
        Message = message;
        StatusCode = (int)statusCode;
        Data = default;
    }
    public static Response<T> Error(string message, HttpStatusCode statusCode)
    {
        return new Response<T>(message, statusCode);
    }
    public static Response<T> Success(T data, string message)
    {
        return new Response<T>(data, message);
    }
}
