using System.Net;
using System.Text.Json.Serialization;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace tavern_api.Commons.Responses;

public sealed class Result<T>
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("data")]
    public T? Data { get; set; } = default;
    [JsonPropertyName("code")]
    public int Code { get; set; }
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; } = false;

    public Result() { }

    private Result(string message, T data, int code, bool isSuccess)
    {
        Data = data;
        Message = message;
        Code = code;
        IsSuccess = isSuccess;
    }

    public Result<T> Success(Result<T> response)
    {
        return new Result<T>(response.Message, response.Data, response.Code, true);
    }

    public Result<T> Failure(Result<T> response)
    {
        return new Result<T>(response.Message, response.Data, response.Code, false);
    }

    public Result<T> Success(string message, T data, int code)
    {
        return new Result<T>(message, data, code, true);
    }

    public Result<T> Failure(string message, T data, int code)
    {
        return new Result<T>(message, default, code, false);
    }
}
