namespace BugTracker.Shared.Helper;

public class ApiResponseHandler<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }

    public ApiResponseHandler(bool success, int statusCode, T data, string message)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public static ApiResponseHandler<T> SuccessResponse(T data = default, string message = null, int statusCode = 200)
    {
        return new ApiResponseHandler<T>(true, statusCode, data, message);
    }

    public static ApiResponseHandler<T> ErrorResponse(string message, T data = default, int statusCode = 400)
    {
        return new ApiResponseHandler<T>(false, statusCode, data, message);
    }
}
