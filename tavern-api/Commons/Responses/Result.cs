using System.Net;

namespace tavern_api.Commons.Responses;

public sealed class Result<T>
{
    public string Message { get; private set; } = string.Empty;
    public T? Data { get; private set; } = default;
    public HttpStatusCode Code { get; private set; } = HttpStatusCode.OK;
    public bool IsSuccess { get; private set; } = false;

    public Result() { }

    private Result(string message, T data, HttpStatusCode code, bool isSuccess)
    {
        Data = data;
        Message = message;
        Code = code;
        IsSuccess = isSuccess;
    }

    public Result<T> Success(string message, T data, HttpStatusCode code = HttpStatusCode.OK)
    {
        return new Result<T>(message, data, code, true);
    }

    public Result<T> Failure(string message, T data, HttpStatusCode code = HttpStatusCode.OK)
    {
        return new Result<T>(message, default, code, false);
    }
}
