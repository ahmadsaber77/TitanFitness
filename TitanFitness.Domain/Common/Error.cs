using System.Net;

namespace TitanFitness.Domain;
public sealed record Error(
    string Code,
    string Message,
    ErrorType Type,
    HttpStatusCode StatusCode)
{

    public static Error NotFound<T>(object key) =>
        new($"{typeof(T).Name}.NotFound", $"{typeof(T).Name} with identifier '{key}' was not found.",
            ErrorType.NotFound, HttpStatusCode.NotFound);

    public static Error Validation<T>(string reason) =>
        new($"{typeof(T).Name}.Validation", reason, ErrorType.Validation, HttpStatusCode.BadRequest);

    public static Error Conflict<T>(string reason) =>
        new($"{typeof(T).Name}.Conflict", reason, ErrorType.Conflict, HttpStatusCode.Conflict);

    public static Error Failure(string reason) =>
        new("General.Failure", reason, ErrorType.Failure, HttpStatusCode.InternalServerError);

    public static Error Unauthorized(string reason) =>
      new("General.Unauthorized", reason,ErrorType.Unauthorized, HttpStatusCode.Unauthorized);
}

public enum ErrorType 
{
    Validation,
    NotFound,
    Conflict,
    Failure,
    Unauthorized
}

