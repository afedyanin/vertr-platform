using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vertr.TinvestGateway;

// https://habr.com/ru/articles/787674/
public class RpcExceptionHandler(IHostEnvironment env, ILogger<RpcExceptionHandler> logger) : IExceptionHandler
{
    private const string UnhandledExceptionMsg = "An unhandled exception has occurred while executing the request.";

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // logger.LogError(exception, exception.Message);

        if (exception is RpcException rpcException)
        {
            var problemDetails = CreateProblemDetails(context, rpcException);
            var json = ToJson(problemDetails);

            const string contentType = "application/problem+json";
            context.Response.ContentType = contentType;
            await context.Response.WriteAsync(json, cancellationToken);

            return true;
        }

        return false;
    }

    // https://developer.tbank.ru/invest/intro/developer/error-codes/errors
    private ProblemDetails CreateProblemDetails(in HttpContext context, in RpcException exception)
    {
        var errorCode = exception.Status.Detail;
        var statusCode = exception.Status.StatusCode;
        // var reasonPhrase = exception.Status.StatusCode.ToString();

        var httpStatusCode = (int)GetResponseStatusCode(exception.Status.StatusCode);
        context.Response.StatusCode = httpStatusCode;
        var reasonPhrase = ReasonPhrases.GetReasonPhrase(httpStatusCode);

        if (string.IsNullOrEmpty(reasonPhrase))
        {
            reasonPhrase = UnhandledExceptionMsg;
        }

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = reasonPhrase,
            Extensions =
            {
                [nameof(errorCode)] = errorCode
            }
        };

        if (!env.IsDevelopment())
        {
            return problemDetails;
        }

        problemDetails.Detail = exception.ToString();
        problemDetails.Extensions["traceId"] = Activity.Current?.Id;
        problemDetails.Extensions["requestId"] = context.TraceIdentifier;
        problemDetails.Extensions["data"] = exception.Data;

        return problemDetails;
    }

    private static HttpStatusCode GetResponseStatusCode(StatusCode code)
        => code switch
        {
            StatusCode.OK => HttpStatusCode.OK,
            StatusCode.Cancelled => HttpStatusCode.InternalServerError,
            StatusCode.Unknown => HttpStatusCode.InternalServerError,
            StatusCode.InvalidArgument => HttpStatusCode.BadRequest,
            StatusCode.DeadlineExceeded => HttpStatusCode.InternalServerError,
            StatusCode.NotFound => HttpStatusCode.NotFound,
            StatusCode.AlreadyExists => HttpStatusCode.InternalServerError,
            StatusCode.PermissionDenied => HttpStatusCode.Forbidden,
            StatusCode.Unauthenticated => HttpStatusCode.Unauthorized,
            StatusCode.ResourceExhausted => HttpStatusCode.BadGateway,
            StatusCode.FailedPrecondition => HttpStatusCode.InternalServerError,
            StatusCode.Aborted => HttpStatusCode.InternalServerError,
            StatusCode.OutOfRange => HttpStatusCode.InternalServerError,
            StatusCode.Unimplemented => HttpStatusCode.InternalServerError,
            StatusCode.Internal => HttpStatusCode.InternalServerError,
            StatusCode.Unavailable => HttpStatusCode.BadGateway,
            StatusCode.DataLoss => HttpStatusCode.InternalServerError,
            _ => throw new NotImplementedException(),
        };

    private string ToJson(in ProblemDetails problemDetails)
    {
        try
        {
            return JsonSerializer.Serialize(problemDetails, SerializerOptions);
        }
        catch (Exception ex)
        {
            const string msg = "An exception has occurred while serializing error to JSON";
            logger.LogError(ex, msg);
        }

        return string.Empty;
    }
}
