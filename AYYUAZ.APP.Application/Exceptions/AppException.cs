using Microsoft.AspNetCore.Http;
using System;

namespace AYYUAZ.APP.Application.Exceptions.AppException
{
    public abstract class AppException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }

        protected AppException(string errorCode, int statusCode) : base()
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }
    }

    public sealed class NotFoundException : AppException
    {
        public NotFoundException() : base("not_found", StatusCodes.Status404NotFound)
        {
        }
    }

    public sealed class BadRequestException : AppException
    {
        public BadRequestException() : base("validation_error", StatusCodes.Status400BadRequest)
        {
        }
    }

    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException() : base("forbidden", StatusCodes.Status403Forbidden)
        {
        }
    }

    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException() : base("unauthorized", StatusCodes.Status401Unauthorized)
        {
        }
    }
}
