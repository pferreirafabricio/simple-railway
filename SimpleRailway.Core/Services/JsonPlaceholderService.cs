using System.Text.Json;
using SimpleRailway.Core.Domain;
using SimpleRailway.Core.Domain.Response;
using SimpleRailway.Core.Extensions;

namespace SimpleRailway.Core.Services;

public class JsonPlaceholderService
{
    private readonly HttpClient client = new();

    public async Task<Result<JsonPlaceholderResponse>> GetDataAsync(string url)
        => await ValidateUrl(url)
            .TryCatch(
                async url => await client.GetAsync(url),
                new(ErrorType.InternalError, "Error calling the API")
            )
            .Bind(async response => ValidateHttpResponse(await response))
            .Bind(async res => ValidateResponseBody(await res.Content.ReadAsStringAsync()))
            .TryCatch(
                body => JsonSerializer.Deserialize<JsonPlaceholderResponse>(body),
                new(ErrorType.InvalidInput, "Error deserializing response body")
            )
            .Bind(data => data is null
                ? Result.Failure<JsonPlaceholderResponse>(new(ErrorType.InvalidInput, "Response body is not in the expected format"))
                : Result.Success(data)
            )
            .Tap(_ => Console.WriteLine($"Passing through the first tap"))
            .Tap(_ => Console.WriteLine($"Passing through the second tap"))
            .Match(
                data => Result.Success(data),
                error => error.Code switch
                {
                    _ => Result.Failure<JsonPlaceholderResponse>(error)
                }
            );

    static Result<string> ValidateUrl(string url)
        => string.IsNullOrWhiteSpace(url)
            ? Result.Failure<string>(new Error(ErrorType.InvalidInput, "Url is empty"))
            : Result.Success(url);

    static Result<HttpResponseMessage> ValidateHttpResponse(HttpResponseMessage httpResponseMessage)
        => httpResponseMessage.IsSuccessStatusCode
            ? Result.Success(httpResponseMessage)
            : Result.Failure<HttpResponseMessage>(new Error(ErrorType.InternalError, httpResponseMessage.ReasonPhrase ?? "Unknown error"));

    static Result<string> ValidateResponseBody(string responseBody)
        => string.IsNullOrWhiteSpace(responseBody)
            ? Result.Failure<string>(new Error(ErrorType.InternalError, "Response body is empty"))
            : Result.Success(responseBody);
}
