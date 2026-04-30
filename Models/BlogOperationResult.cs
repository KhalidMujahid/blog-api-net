namespace blog_api.Models;

public record BlogOperationResult<T>(T? Value, OperationStatus Status)
{
    public static BlogOperationResult<T> Success(T value) => new(value, OperationStatus.Success);
    public static BlogOperationResult<T> NotFound => new(default, OperationStatus.NotFound);
    public static BlogOperationResult<T> Forbidden => new(default, OperationStatus.Forbidden);
}

public enum OperationStatus
{
    Success,
    NotFound,
    Forbidden
}

public enum DeleteStatus
{
    Success,
    NotFound,
    Forbidden
}
