using Microsoft.AspNetCore.Diagnostics;
using PortfolioHub.Application.Exceptions;
using PortfolioHub.Domain.Exceptions;
using PortfolioHub.Domain.Exceptions.ValueObjects;

namespace PortfolioHub.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is WalletNotFoundException)
            return await WriteProblemAsync(
                httpContext,
                StatusCodes.Status404NotFound,
                "Carteira não encontrada",
                exception.Message);

        if (exception is AssetNotFoundException)
            return await WriteProblemAsync(
                httpContext,
                StatusCodes.Status404NotFound,
                "Ativo não encontrado",
                exception.Message);

        if (exception is InsufficientBalanceException
            or InsufficientAssetQuantityException)
            return await WriteProblemAsync(
                httpContext,
                StatusCodes.Status409Conflict,
                "Operação não permitida",
                exception.Message);

        if (exception is BaseException
            or ArgumentException)
            return await WriteProblemAsync(
                httpContext,
                StatusCodes.Status400BadRequest,
                "Dados inválidos",
                exception.Message);

        return false;
    }

    private static async ValueTask<bool> WriteProblemAsync(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail)
    {
        await Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail)
            .ExecuteAsync(httpContext);

        return true;
    }
}