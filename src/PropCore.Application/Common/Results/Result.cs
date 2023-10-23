namespace PropCore.Application.Common.Results;

public interface IResult
{
    bool IsSuccess { get; }

    string? Error { get; }
}

public sealed record Result : IResult
{
    public bool IsSuccess { get; init; }

    public string? Error { get; init; }

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
}

public sealed record Result<T> : IResult
{
    public bool IsSuccess { get; init; }

    public string? Error { get; init; }

    public T? Value { get; init; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };

    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}